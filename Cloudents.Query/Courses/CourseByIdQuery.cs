﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Courses
{
    public class CourseByIdQuery : IQuery<CourseDetailDto?>
    {
        private long Id { get; }

        private long UserId { get; }

        public CourseByIdQuery(long id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        internal sealed class CourseByIdQueryHandler : IQueryHandler<CourseByIdQuery, CourseDetailDto?>
        {
            private readonly IStatelessSession _statelessSession;
            private readonly ICronService _cronService;
            private readonly IUrlBuilder _urlBuilder;

            public CourseByIdQueryHandler(IStatelessSession repository, ICronService cronService,
                IUrlBuilder urlBuilder)
            {
                _statelessSession = repository;
                _cronService = cronService;
                _urlBuilder = urlBuilder;
            }

            public async Task<CourseDetailDto?> GetAsync(CourseByIdQuery query, CancellationToken token)
            {
                var futureStudyRoom = _statelessSession.Query<Course>()
                    .Where(w => w.Id == query.Id)
                    .Select(s => new CourseDetailDto
                    {
                        Documents = s.Documents.Where(w => w.Status.State == ItemState.Ok).Select(s2 =>
                            new DocumentFeedDto()
                            {
                                Title = s2.Name,
                                DocumentType = s2.DocumentType,
                                Id = s2.Id,
                                Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(s2.Id, null)
                            }),
                        Id = s.Id,
                        StudyRooms = s.StudyRooms.Where(w => w.BroadcastTime > DateTime.UtcNow.AddHours(-1)).Select(
                            s2 => new FutureBroadcastStudyRoomDto()
                            {
                                Id = s2.Id,
                                DateTime = s2.BroadcastTime,
                                // Description = s2.Description,
                                //IsFull = _statelessSession.Query<StudyRoomUser>().Count(w => w.Room.Id == s2.Id) >= 48,
                                //Enrolled = _statelessSession.Query<StudyRoomUser>()
                                //    .Any(w => w.Room.Id == s2.Id && w.User.Id == query.UserId),
                                Schedule = s2.Schedule
                            }),
                        Enrolled = s.CourseEnrollments.Any(a => a.User.Id == query.UserId),
                        TutorId = s.Tutor.Id,
                        //BroadcastTime = s.BroadcastTime,
                        Name = s.Name,
                        Price = s.Price,
                        SubscriptionPrice = s.SubscriptionPrice,
                        TutorName = s.Tutor.User.Name,
                        TutorImage = s.Tutor.User.ImageName,
                        TutorCountry = s.Tutor.User.SbCountry,
                        Description = s.Description,
                        Full = s.CourseEnrollments.Count() == 48,
                        TutorBio = s.Tutor.Paragraph2,

                        //SessionStarted =  _statelessSession.Query<StudyRoomSession>().Any(w=>w.StudyRoom.Id== query.Id && w.Ended ==null)
                    }).ToFutureValue();

                var futureSubscription = _statelessSession.Query<Follow>()
                    .Where(w => w.User.Id == _statelessSession.Query<Course>()
                        .Where(w2 => w2.Id == query.Id)
                        .Select(s => s.Tutor.Id).First())
                    .Where(w => w.Follower.Id == query.UserId)
                    .Select(s => s.Subscriber)
                    .ToFutureValue();

                var result = await futureStudyRoom.GetValueAsync(token);

                if (result is null)
                {
                    return null;
                }

                //if (result.BroadcastTime < DateTime.UtcNow.AddHours(-6))
                //{
                //    return null;
                //}

                result.StudyRooms = result.StudyRooms.Select(s =>
                {
                    if (s.Schedule != null)
                    {
                        s.NextEvents = _cronService.GetNextOccurrences(s.Schedule.CronString,
                            s.Schedule.Start, s.Schedule.End);
                    }

                    return s;
                });




                if (futureSubscription.Value.GetValueOrDefault())
                {
                    result.Price = result.SubscriptionPrice.GetValueOrDefault(result.Price.ChangePrice(0));
                }

                return result;
            }
        }
    }
}