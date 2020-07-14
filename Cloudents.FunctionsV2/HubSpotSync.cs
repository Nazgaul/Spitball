
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
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
        private static readonly HubSpotClient _client = new HubSpotClient();

        [FunctionName("HubSpotSync")]
        public static async Task RunOrchestratorAsync(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            CancellationToken token)
        {
            //Inject Istateless session comes when it already dispose - need to figure it out this is a work out
            //using var child = lifetimeScope.BeginLifetimeScope();
            //var statelessSession =  child.Resolve<IStatelessSession>();
            var amountOfTutors = await context.CallActivityAsync<int>("HubSpotSync_GetCount",null);

            for (int i = 0; i < amountOfTutors / 100; i++)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                await context.CallActivityAsync("HubSpotSync_DoSync", i);
            }
        }

        [FunctionName("HubSpotSync_GetCount")]
        public static async Task<int> GetCountAsync([ActivityTrigger] string x,
            [Inject] IStatelessSession statelessSession,
            ILogger log, CancellationToken token)
        {
            var amountOfTutors = await statelessSession.Query<Tutor>().Fetch(f => f.User)
                .Where(w => w.User.SbCountry == Country.UnitedStates)
                .CountAsync(cancellationToken: token);
            return amountOfTutors;
        }
      


        [FunctionName("HubSpotSync_DoSync")]
        public static async Task DoSyncAsync([ActivityTrigger] int index,
            [Inject] IStatelessSession statelessSession,
            ILogger log)
        {
            log.LogInformation($"Processing index {index}");
            var tutors = await statelessSession.Query<Tutor>().Fetch(f => f.User)
                    .Where(w => w.User.SbCountry == Country.UnitedStates)
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
                        Courses = statelessSession.Query<UserCourse>().Any(w => w.User.Id == s.Id),
                        StudyRoom = statelessSession.Query<StudyRoom>().Any(a => a.Tutor.Id == s.Id),
                        Documents = statelessSession.Query<Core.Entities.Document>().Count(w2 =>  w2.User.Id == s.Id),
                        Followers = statelessSession.Query<Follow>().Count(c => c.User.Id == s.Id),
                        Lessons = statelessSession.Query<StudyRoomSession>().Count(c => c.StudyRoom.Tutor.Id == s.Id),
                    }).OrderBy(o => o.Id).Take(100).Skip(100 * index)
                    .ToListAsync();
            foreach (var tutor in tutors)
            {
                var contact = await _client.GetContactByEmailAsync(tutor.Email);

                IFutureEnumerable<string> coursesFuture = null;

                var average = 0f;
                IFutureEnumerable<StudyRoomDto> studyRoomFuture = null;
                if (tutor.Courses)
                {
                    coursesFuture = statelessSession.Query<UserCourse>().Where(w => w.User.Id == tutor.Id).Select(s => s.Course.Id)
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
                IEnumerable<string> courses;
                if (coursesFuture != null)
                {
                    courses = await coursesFuture?.GetEnumerableAsync();
                }
                else
                {
                    courses = new List<string>();
                }

                List<StudyRoomDto> studyRoomData;
                if (studyRoomFuture != null)
                {
                    studyRoomData = (await studyRoomFuture?.GetEnumerableAsync()).ToList();
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
                //.contact.
                contact.LiveDone = studyRoomData.Count(w =>
                    w.Type == StudyRoomType.Broadcast && w.BroadcastTime < DateTime.UtcNow);
                contact.LiveScheduled = studyRoomData.Count(w =>
                    w.Type == StudyRoomType.Broadcast && w.BroadcastTime >= DateTime.UtcNow);


                contact.PrivateDone = contact.PrivateScheduled =
                    studyRoomData.Count(w => w.Type == StudyRoomType.Private);

                await _client.CreateOrUpdateAsync(contact, needInsert);
            }


        }

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
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("HubSpotSync", "HubSpotSync2");
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        }


        private class StudyRoomDto
        {
            public StudyRoomDto(StudyRoomType type, DateTime? broadcastTime, int count)
            {
                Type = type;
                BroadcastTime = broadcastTime;
                Count = count;
            }

            public StudyRoomType Type { get; set; }
            public DateTime? BroadcastTime { get; set; }

            public int Count { get; set; }
        }
    }
}