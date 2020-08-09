using System;
using System.Linq;
using Cloudents.Core.DTOs;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Tutor
{
    public class TutorListQuery : IQuery<ListWithCountDto<TutorCardDto>>
    {
        public TutorListQuery(long userId, Country country, int page, int pageSize = 20)
        {
            UserId = userId;
            Country = country ?? throw new ArgumentNullException(nameof(country));
            Page = page;
            PageSize = pageSize;
        }


        private long UserId { get; }
        private Country Country { get; }
        private int Page { get; }
        private int PageSize { get;  }

        internal sealed class TutorListQueryHandler : IQueryHandler<TutorListQuery, ListWithCountDto<TutorCardDto>>
        {
            private readonly IStatelessSession _statelessSession;
            private readonly IUrlBuilder _urlBuilder;

            public TutorListQueryHandler(IStatelessSession dapper, IUrlBuilder urlBuilder)
            {
                _statelessSession = dapper;
                _urlBuilder = urlBuilder;
            }

            public async Task<ListWithCountDto<TutorCardDto>> GetAsync(TutorListQuery query, CancellationToken token)
            {
               var dbQuery =  _statelessSession.Query<Core.Entities.ReadTutor>()
                    .Where(w=>w.SbCountry == query.Country &&
                              w.State == ItemState.Ok && w.Id != query.UserId);


               var futureResult = dbQuery.OrderBy(o => o.Rate)
                   .Take(query.PageSize).Skip(query.Page * query.PageSize)
                   .Select(s => new TutorCardDto
                   {
                       Name = s.Name,
                       Bio = s.Bio,
                       Image = s.ImageName,
                       UserId = s.Id,
                       Lessons = s.Lessons,
                       Rate = s.Rate,
                       Courses = s.Courses,
                       ReviewsCount = s.RateCount
                   }).ToFuture();

               var futureCount = dbQuery.ToFutureValue(f => f.Count());
                
              
                var tutor = await futureResult.GetEnumerableAsync(token);

                var count = futureCount.Value;
                return new ListWithCountDto<TutorCardDto>()
                {
                    Count = count,
                    Result = tutor.Select(s =>
                    {
                        s.Image = _urlBuilder.BuildUserImageEndpoint(s.UserId,s.Image);
                        return s;
                    })
                };
            }
        }
    }



}
