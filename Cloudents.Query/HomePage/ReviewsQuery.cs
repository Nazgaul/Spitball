using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.HomePage
{
    public class ReviewsQuery : IQuery<IEnumerable<ReviewDto>>
    {
        public ReviewsQuery(Country country, int count)
        {
            Country = country;
            Count = count;
        }

        private Country Country { get; }
        private int Count { get; }

        internal sealed class TopTutorsQueryHandler : IQueryHandler<ReviewsQuery, IEnumerable<ReviewDto>>
        {
            private readonly IStatelessSession _session;

            public TopTutorsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            [Cache(TimeConst.Day, "homePage-reviews3", false)]
            //We cant put cache due to serialize issue
            public async Task<IEnumerable<ReviewDto>> GetAsync(ReviewsQuery query, CancellationToken token)
            {
                return await _session.Query<TutorReview>()
                    .WithOptions(w => w.SetComment(nameof(ReviewsQuery)))
                    .Fetch(f => f.User)
                    .Join(_session.Query<ReadTutor>(), review => review.Tutor.Id, read => read.Id, (review, tutor) =>
                        new
                        {
                            review,
                            tutor
                        })
                    .Where(w => w.review.IsShownHomePage && w.tutor.Country == query.Country.ToString())
                        .Select(s => new ReviewDto()
                        {
                            Text = s.review.Review,
                            UserName = s.review.User.Name,
                            TutorImage = s.tutor.ImageName,
                            TutorName = s.tutor.Name,
                            TutorReviews = s.tutor.Rate.GetValueOrDefault(),
                            TutorCount = s.tutor.RateCount,
                            TutorId = s.tutor.Id



                        }).Take(query.Count).ToListAsync(token);
            }
        }
    }




    public class ReviewDto
    {
        public string Text { get; set; }
        public string UserName { get; set; }
        public string TutorImage { get; set; }
        public string TutorName { get; set; }

        public double TutorReviews { get; set; }

        public long TutorId { get; set; }
        public int TutorCount { get; set; }
    }
}