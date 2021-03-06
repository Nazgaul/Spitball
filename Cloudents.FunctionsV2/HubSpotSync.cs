
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class HubSpotSync
    {
        private static readonly HubSpotClient Client = new HubSpotClient();
      

        [FunctionName("HubSpotSync")]
        public static async Task RunOrchestratorAsync(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            CancellationToken token)
        {
            var amountOfTutors = await context.CallActivityAsync<int>("HubSpotSync_GetCount", null);

            for (int i = 0; i <= amountOfTutors / 100; i++)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                await context.CallActivityAsync("HubSpotSync_DoSync", i);
            }

            //await context.CallActivityAsync("HubSpotSync_FinishProcess",amountOfTutors);
        }


        //[FunctionName("HubSpotSync_FinishProcess")]
        //public static async Task SendEmailAsync([ActivityTrigger] int amountOfTutors,
        //    [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<SendGridMessage> emailProvider,
        //    CancellationToken token)
        //{
        //    var message = new SendGridMessage()
        //    {

        //        Subject = "Finish Sync Hubspot",
        //        PlainTextContent = $"Finish process {amountOfTutors}"
        //    };
        //    message.AddTo("ram@cloudents.com");
        //    await emailProvider.AddAsync(message, token);
        //}

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [FunctionName("HubSpotSync_GetCount")]
        public static async Task<int> GetCountAsync([ActivityTrigger] string x,
            [Inject] IStatelessSession statelessSession,
            ILogger log, CancellationToken token)
        {
            var amountOfTutors = await statelessSession.Query<Tutor>().Fetch(f => f.User)
                .Where(w => w.User.SbCountry == Country.UnitedStates  && w.State != ItemState.Flagged)
                .CountAsync(cancellationToken: token);
            return amountOfTutors;
        }


        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [FunctionName("HubSpotSync_DoSync")]
        public static async Task DoSyncAsync([ActivityTrigger] int index,
            [Inject] IStatelessSession statelessSession,
            ILogger log)
        {
            log.LogInformation($"Processing index {index}");
            var tutors = await statelessSession.Query<Tutor>().Fetch(f => f.User)
                    .Where(w => w.User.SbCountry == Country.UnitedStates && w.State != ItemState.Flagged)
                    .Select(s => new
                    {
                        s.User.Email,
                        s.User.FirstName,
                        s.User.LastName,
                        s.Id,
                        s.User.PhoneNumber,
                        RegistrationDate = s.User.Created,
                        s.User.Country,
                        s.State,
                        s.Paragraph2,
                        s.Paragraph3,
                        s.Title,
                        s.SellerKey,
                        s.SubscriptionPrice,
                        Courses = statelessSession.Query<Core.Entities.Course>().Any(w => w.Tutor.Id == s.Id),
                        StudyRoom = statelessSession.Query<StudyRoom>().Any(a => a.Tutor.Id == s.Id),
                        Documents = statelessSession.Query<Core.Entities.Document>().Count(w2 => w2.User.Id == s.Id),
                        Followers = statelessSession.Query<Follow>().Count(c => c.User.Id == s.Id),
                        Lessons = statelessSession.Query<StudyRoomSession>().Count(c => c.StudyRoom.Tutor.Id == s.Id),
                    }).OrderBy(o => o.Id).Take(100).Skip(100 * index)
                    .ToListAsync();
            foreach (var tutor in tutors)
            {
                
                var contact = await Client.GetContactByEmailAsync(tutor.Email);

                IFutureEnumerable<string>? coursesFuture = null;

                var average = 0f;
                IFutureEnumerable<StudyRoomDto>? studyRoomFuture = null;
                if (tutor.Courses)
                {
                    coursesFuture = statelessSession.Query<Core.Entities.Course>().Where(w => w.Tutor.Id == tutor.Id).Select(s => s.Name)
                        .ToFuture();
                }
                if (tutor.StudyRoom)
                {


                    studyRoomFuture = statelessSession.Query<StudyRoom>()
                        .Where(w => w.Tutor.Id == tutor.Id)
                        .Select(s => new StudyRoomDto
                        (
                            (s is BroadCastStudyRoom) ? StudyRoomType.Broadcast : StudyRoomType.Private,
                            (s is BroadCastStudyRoom) ? ((BroadCastStudyRoom)s).BroadcastTime : new DateTime?(),
                            s.Sessions.Count()
                        )).ToFuture();
                }
                if (tutor.Lessons > 0)
                {
                    var query = from e in statelessSession.Query<TutorReview>()
                                where e.Tutor.Id == tutor.Id
                                group e by 1 into grp
                                select new
                                {
                                    Count = grp.Count(),
                                    //Average is doing issue - doing average in c# 
                                    Sum = grp.Sum(x => (float?)x.Rate)
                                };
                    var reviewsFutureValue = query.ToFutureValue();
                    var reviewCount = reviewsFutureValue.Value?.Count ?? 0;
                    if (reviewCount > 0)
                    {
                        average = (reviewsFutureValue.Value?.Sum ?? 0) / reviewCount;
                    }
                }
                IEnumerable<string>? courses;
                if (coursesFuture != null)
                {
                    courses = await coursesFuture.GetEnumerableAsync();
                }
                else
                {
                    courses = new List<string>();
                }

                List<StudyRoomDto> studyRoomData;
                if (studyRoomFuture != null)
                {
                    studyRoomData = (await studyRoomFuture.GetEnumerableAsync()).ToList();
                }
                else
                {
                    studyRoomData = new List<StudyRoomDto>();
                }
                var needInsert = false;
                if (contact == null)
                {
                    needInsert = true;
                    contact = new HubSpotContact { Email = tutor.Email.Trim() };
                }

                contact.SpitballId = tutor.Id;
                if (string.IsNullOrEmpty(contact.FirstName))
                {
                    contact.FirstName = tutor.FirstName;
                }
                if (string.IsNullOrEmpty(contact.Lastname))
                {
                    contact.Lastname = tutor.LastName?.Replace(".", " ");
                }
                if (string.IsNullOrEmpty(contact.Phone))
                {
                    contact.Phone = tutor.PhoneNumber;
                }
                if (string.IsNullOrEmpty(contact.Website) || contact.Website.Contains("spitball.co"))
                {

                    contact.Website = $"https://www.spitball.co/p/{tutor.Id}";
                }

                if (string.IsNullOrEmpty(contact.LeadStatus))
                {
                    contact.LeadStatus = "NEW";
                }

                if (string.IsNullOrEmpty(contact.OwnerId))
                {
                    contact.OwnerId = "49087036";
                }

                contact.SetRegistrationTime(tutor.RegistrationDate);
                contact.Bio = $"{tutor.Paragraph2} {tutor.Paragraph3}";
                contact.Title = tutor.Title;
                contact.Payment = tutor.SellerKey != null;
                contact.SubscriptionPrice = tutor.SubscriptionPrice?.Amount;
                contact.Country = tutor.Country;
                contact._status = tutor.State;
                contact.DocumentCount = tutor.Documents;
                contact.FollowerCount = tutor.Followers;
                contact.LessonsCount = tutor.Lessons;
                contact.Courses = string.Join(", ", courses);

                contact.Rate = average;
                contact.LiveDone = studyRoomData.Count(w =>
                    w.Type == StudyRoomType.Broadcast && w.BroadcastTime < DateTime.UtcNow);
                contact.LiveScheduled = studyRoomData.Count(w =>
                    w.Type == StudyRoomType.Broadcast && w.BroadcastTime >= DateTime.UtcNow);


                contact.PrivateDone = contact.PrivateScheduled =
                    studyRoomData.Count(w => w.Type == StudyRoomType.Private);

                contact.TutorState = contact.LiveDone == 0 && contact.LiveScheduled == 0 ? "opportunity" : "customer";
                await Client.CreateOrUpdateAsync(contact, needInsert);
            }


        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [FunctionName("HubSpotSync_TimerStart")]
        public static async Task TimerStartAsync(
            [TimerTrigger("0 0 */12 * * *")] TimerInfo time,
            [DurableClient] IDurableOrchestrationClient starter,
            [Inject] IConfigurationKeys configuration,
            ILogger log)
        {
            if (configuration.Search.IsDevelop)
            {
                return;
            }

            const string instanceId = "HubSpotSync3";
            DurableOrchestrationStatus? status;
            do
            {
                status  = await starter.GetStatusAsync(instanceId);
                if (status?.RuntimeStatus == OrchestrationRuntimeStatus.Running)
                {
                    await starter.TerminateAsync(instanceId, "new run");
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }

            } while (status?.RuntimeStatus == OrchestrationRuntimeStatus.Running);
            string instanceId2 = await starter.StartNewAsync("HubSpotSync", instanceId);
            log.LogInformation($"Started orchestration with ID = '{instanceId2}'.");
        }


        private class StudyRoomDto
        {
            public StudyRoomDto(StudyRoomType type, DateTime? broadcastTime, int count)
            {
                Type = type;
                BroadcastTime = broadcastTime;
                //  Count = count;
            }

            public StudyRoomType Type { get; }
            public DateTime? BroadcastTime { get; }

            // private int Count { get; }
        }
    }
}