//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Enum;
//using Cloudents.Infrastructure;
//using NHibernate;
//using NHibernate.Linq;
//using Skarp.HubSpotClient.Contact;
//using Skarp.HubSpotClient.Contact.Dto;

//namespace ConsoleApp
//{

   
//    public class HubSpotSync
//    {
//        private readonly IStatelessSession _statelessSession;
//        private readonly HubSpotClient _client = new HubSpotClient();


//        public HubSpotSync(IStatelessSession statelessSession)
//        {
//            _statelessSession = statelessSession;
//        }


//        public async Task Sync()
//        {

           

//            var page = 0;
//            int count;
//            do
//            {
//                var tutors = await _statelessSession.Query<Tutor>().Fetch(f => f.User)
//                    .Where(w => w.User.SbCountry == Country.UnitedStates)
//                    .Select(s => new
//                    {
//                        s.User.Email,
//                        s.User.FirstName,
//                        s.User.LastName,
//                        s.Id,
//                        s.User.PhoneNumber,
//                        RegistrationDate = s.User.Created,
//                        s.User.Country,
//                        s.State,
//                        s.Paragraph2,
//                        s.Paragraph3,
//                        s.Title,
//                        s.SellerKey,
//                        s.SubscriptionPrice,
//                        Courses = _statelessSession.Query<UserCourse>().Any(w => w.User.Id == s.Id),
//                        StudyRoom = _statelessSession.Query<StudyRoom>().Any(a=>a.Tutor.Id == s.Id),
//                        Documents = _statelessSession.Query<Document>().Count(w => w.User.Id == s.Id),
//                        Followers = _statelessSession.Query<Follow>().Count(c => c.User.Id == s.Id),
//                        Lessons = _statelessSession.Query<StudyRoomSession>().Count(c => c.StudyRoom.Tutor.Id == s.Id),
//                    }).OrderBy(o => o.Id).Take(100).Skip(100 * page)

//                    .ToListAsync();
//                page++;
//                count = tutors.Count;
//                foreach (var tutor in tutors)
//                {
//                    Console.WriteLine("processing " + tutor.Id);
//                   var contact = await _client.GetContactByEmailAsync(tutor.Email);

//                    IFutureEnumerable<string> coursesFuture = null;

//                    var average = 0f;
//                    IFutureEnumerable<StudyRoomDto> studyRoomFuture = null;
//                    if (tutor.Courses)
//                    {
//                        coursesFuture = _statelessSession.Query<UserCourse>().Where(w => w.User.Id == tutor.Id).Select(s => s.Course.Id)
//                            .ToFuture();
//                    }
//                    else
//                    {
//                        Console.WriteLine("no courses");
//                        //
//                    }
//                    if (tutor.StudyRoom)
//                    {
                       

//                        studyRoomFuture = _statelessSession.Query<StudyRoom>()
//                            .Where(w => w.Tutor.Id == tutor.Id)
//                            .Select(s => new StudyRoomDto
//                            (
//                                (s is BroadCastStudyRoom) ? StudyRoomType.Broadcast : StudyRoomType.Private,
//                                (s is BroadCastStudyRoom) ? ((BroadCastStudyRoom)s).BroadcastTime : new DateTime?(),
//                                s.Sessions.Count()
//                            )).ToFuture();
                       
//                    }
//                    else
//                    {
//                        Console.WriteLine("no rooms");
//                    }

//                    if (tutor.Lessons > 0)
//                    {
//                        var query = from e in _statelessSession.Query<TutorReview>()
//                                    where e.Tutor.Id == tutor.Id
//                                    group e by 1 into grp
//                                    select new
//                                    {
//                                        Count = grp.Count(),
//                                        //Average is doing issue - doing average in c# 
//                                        Sum = grp.Sum(x => (float?)x.Rate)
//                                    };
//                        var reviewsFutureValue = query.ToFutureValue();
//                        var reviewCount = reviewsFutureValue.Value?.Count ?? 0;
//                        if (reviewCount > 0)
//                        {
//                            average = (reviewsFutureValue.Value?.Sum ?? 0) / reviewCount;
//                        }
//                    }
//                    else
//                    {
//                        Console.WriteLine("no lessons");
//                    }

//                    IEnumerable<string> courses;
//                    if (coursesFuture != null)
//                    {
//                        courses = await coursesFuture?.GetEnumerableAsync();
//                    }
//                    else
//                    {
//                        courses = new List<string>();
//                    }

//                    List<StudyRoomDto> studyRoomData;
//                    if (studyRoomFuture != null)
//                    {
//                        studyRoomData = (await studyRoomFuture?.GetEnumerableAsync()).ToList();
//                    }
//                    else
//                    {
//                        studyRoomData = new List<StudyRoomDto>();
//                    }
//                    var needInsert = false;
//                    if (contact == null)
//                    {
//                        needInsert = true;
//                        contact = new HubSpotExtra { Email = tutor.Email.Trim() };
//                    }

//                    contact.SpitballId = tutor.Id;
//                    if (string.IsNullOrEmpty(contact.FirstName))
//                    {
//                        contact.FirstName = tutor.FirstName;
//                    }
//                    if (string.IsNullOrEmpty(contact.Lastname))
//                    {
//                        contact.Lastname = tutor.LastName?.Replace(".", " ");
//                    }
//                    if (string.IsNullOrEmpty(contact.Phone))
//                    {
//                        contact.Phone = tutor.PhoneNumber;
//                    }
//                    if (string.IsNullOrEmpty(contact.Website) || contact.Website.Contains("spitball.co"))
//                    {

//                        contact.Website = $"https://www.spitball.co/p/{tutor.Id}";
//                    }

//                    if (string.IsNullOrEmpty(contact.OwnerId))
//                    {
//                        contact.OwnerId = "49087036";
//                    }

//                    contact.SetRegistrationTime(tutor.RegistrationDate);
//                    contact.Bio = $"{tutor.Paragraph2} {tutor.Paragraph3}";
//                    contact.Title = tutor.Title;
//                    contact.Payment = tutor.SellerKey != null;
//                    contact.SubscriptionPrice = tutor.SubscriptionPrice?.Amount;
//                    contact.Country = tutor.Country;
//                    contact._status = tutor.State;
//                    contact.DocumentCount = tutor.Documents;
//                    contact.FollowerCount = tutor.Followers;
//                    contact.LessonsCount = tutor.Lessons;
//                    contact.Courses = string.Join(", ", courses);
                    
//                    contact.Rate = average;
//                    //.contact.
//                    contact.LiveDone = studyRoomData.Count(w =>
//                        w.Type == StudyRoomType.Broadcast && w.BroadcastTime < DateTime.UtcNow);
//                    contact.LiveScheduled = studyRoomData.Count(w =>
//                        w.Type == StudyRoomType.Broadcast && w.BroadcastTime >= DateTime.UtcNow);


//                    contact.PrivateDone = contact.PrivateScheduled =
//                        studyRoomData.Count(w => w.Type == StudyRoomType.Private);

//                    await _client.CreateOrUpdateAsync(contact, needInsert);
//                }

//            } while (count > 0);

//        }

//    }

   


   
//}
