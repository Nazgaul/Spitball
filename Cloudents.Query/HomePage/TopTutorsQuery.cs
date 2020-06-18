﻿using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;

namespace Cloudents.Query.HomePage
{
    public class TopTutorsQuery : IQuery<IEnumerable<TutorCardDto>>
    {
        public TopTutorsQuery(Country country, int count)
        {
            Country = country;
            Count = count;
        }

        private Country Country { get; }
        private int Count { get; }

        internal sealed class TopTutorsQueryHandler : IQueryHandler<TopTutorsQuery, IEnumerable<TutorCardDto>>
        {
            private readonly IStatelessSession _session;

            public TopTutorsQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            [Cache(TimeConst.Day, "homePage-tutors", false)]
            //We cant put cache due to serialize issue
            public async Task<IEnumerable<TutorCardDto>> GetAsync(TopTutorsQuery query, CancellationToken token)
            {
                var linqQuery = _session.Query<ReadTutor>()
                    .WithOptions(w => w.SetComment(nameof(TopTutorsQuery)))
                    .Join(_session.Query<Core.Entities.Tutor>(), readTutor => readTutor.Id, tutor => tutor.Id,
                        (readTutor, tutor) =>
                            new
                            {
                                readTutor,
                                tutor
                            });
                linqQuery = linqQuery.Where(w => w.readTutor.SbCountry == query.Country);

                return await linqQuery.Where(w => w.tutor.IsShownHomePage && w.tutor.State == Core.Enum.ItemState.Ok)
                    .OrderByDescending(o => o.readTutor.OverAllRating)
                    .Select(s => new TutorCardDto()
                {
                    UserId = s.readTutor.Id,
                    Name = s.readTutor.Name,
                    Image = s.readTutor.ImageName,
                    Rate = s.readTutor.Rate,
                    Bio = s.readTutor.Bio,
                    Courses = s.readTutor.Courses,
                    Lessons = s.readTutor.Lessons,
                    ReviewsCount = s.readTutor.RateCount,
                    Subjects = s.readTutor.Subjects,
                }).Take(query.Count).ToListAsync(token);
               

            }
        }
    }
}
