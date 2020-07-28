using System;
using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Users
{
    public class UserCoursesQuery : IQuery<IEnumerable<CourseDto>>
    {
        public UserCoursesQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; set; }

        internal sealed class UserCoursesQueryHandler : IQueryHandler<UserCoursesQuery, IEnumerable<CourseDto>>
        {
            private readonly IStatelessSession _statelessSession;
            private readonly IUrlBuilder _urlBuilder;
            private readonly ICronService _cronService;

            public UserCoursesQueryHandler(IStatelessSession statelessSession, IUrlBuilder urlBuilder, ICronService cronService)
            {
                _urlBuilder = urlBuilder;
                _cronService = cronService;
                _statelessSession = statelessSession;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(UserCoursesQuery query, CancellationToken token)
            {



                var result =  await _statelessSession.Query<Course>()
                    .Where(w => w.Tutor.Id == query.UserId && w.State == ItemState.Ok)
                    .Select(s => new CourseDto
                    {
                        Name = s.Name,
                        Price = s.Price,
                        SubscriptionPrice = s.SubscriptionPrice,

                        //Documents = s.Documents.Where(w=>w.Status.State == ItemState.Ok).Select(s2 => new DocumentFeedDto()
                        //{
                        //    Title = s2.Name,
                        //    DocumentType = s2.DocumentType,
                        //    Id = s2.Id,
                        //    Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(s2.Id, null)
                        //}),
                        //Id = s.Id,
                        //StudyRooms = s.StudyRooms.Where(w=> w.BroadcastTime > DateTime.UtcNow.AddHours(-1)).Select(s2 => new FutureBroadcastStudyRoomDto()
                        //{
                        //    Id = s2.Id,
                        //    DateTime = s2.BroadcastTime,
                        //    Description = s2.Description,
                        //    //IsFull = _statelessSession.Query<StudyRoomUser>().Count(w => w.Room.Id == s2.Id) >= 48,
                        //    //Enrolled = _statelessSession.Query<StudyRoomUser>()
                        //    //    .Any(w => w.Room.Id == s2.Id && w.User.Id == query.UserId),
                        //    Schedule = s2.Schedule
                        //})
                    }).ToListAsync(token);

                return result.Select(s =>
                {
                    s.StudyRooms = s.StudyRooms.Select(s2 =>
                    {
                        if (s2.Schedule != null)
                        {
                            s2.NextEvents = _cronService.GetNextOccurrences(s2.Schedule.CronString,
                                s2.Schedule.Start, s2.Schedule.End);
                        }

                        return s2;
                    });
                    return s;
                });


            }
        }
    }
}
