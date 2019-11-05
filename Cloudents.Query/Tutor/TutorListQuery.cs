using Cloudents.Core.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Cloudents.Query.Tutor
{
    public class TutorListQuery : IQuery<IEnumerable<TutorCardDto>>
    {
        public TutorListQuery(long userId, string country, int page, int pageSize = 20)
        {
            UserId = userId;
            Country = country;
            Page = page;
            PageSize = pageSize;
        }


        private long UserId { get; }
        private string Country { get; }
        private int Page { get; }
        public int PageSize { get; set; }

        internal sealed class TutorListQueryHandler : IQueryHandler<TutorListQuery, IEnumerable<TutorCardDto>>
        {
            private readonly IStatelessSession _session;

            public TutorListQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            //TODO: review query 
            public Task<IEnumerable<TutorCardDto>> GetAsync(TutorListQuery query, CancellationToken token)
            {
                //TODO maybe we can fix this query
                ReadTutor viewTutorAlias = null;
                UserCourse userCourseAlias = null;
                Course courseAlias = null;


                var listOfQueries = new List<IQueryOver<ReadTutor, ReadTutor>>();
                IQueryOver<ReadTutor, ReadTutor> futureCourse = _session.QueryOver(() => viewTutorAlias).Where(w => w.Id != query.UserId);

                listOfQueries.Add(futureCourse);
                if (!string.IsNullOrEmpty(query.Country))
                {
                    futureCourse.Where(() => viewTutorAlias.Country == query.Country);
                }

                if (query.UserId > 0)
                {
                    var withCountryOnlyDetachedQuery = futureCourse.Clone();

                    var detachedQuery = QueryOver.Of(() => viewTutorAlias)
                        .JoinEntityAlias(() => userCourseAlias, () => userCourseAlias.User.Id == viewTutorAlias.Id)
                        .Where(() => userCourseAlias.CanTeach)



                        .WithSubquery.WhereProperty(() => userCourseAlias.Course.Id).In(

                            QueryOver.Of<UserCourse>()
                                .Where(w => w.User.Id == query.UserId).Select(s => s.Course.Id)
                                )


                        .Select(s => s.Id);


                    var detachedQuery2 = QueryOver.Of(() => viewTutorAlias)
                        .JoinEntityAlias(() => userCourseAlias, () => userCourseAlias.User.Id == viewTutorAlias.Id)
                        .JoinAlias(() => userCourseAlias.Course, () => courseAlias)
                        .Where(() => userCourseAlias.CanTeach)
                        .WithSubquery.WhereProperty(() => courseAlias.Subject.Id).In(
                            QueryOver.Of<Course>()
                                .JoinQueryOver(x => x.Users)
                                .Where(w => w.User.Id == query.UserId).Select(s => s.Subject.Id))
                        .Select(s => s.Id);
                    var futureCourse2 = futureCourse.Clone().WithSubquery.WhereProperty(w => w.Id).NotIn(detachedQuery);

                    listOfQueries.Add(futureCourse2);



                    futureCourse.WithSubquery.WhereProperty(w => w.Id).In(detachedQuery);
                    futureCourse2.WithSubquery.WhereProperty(w => w.Id).In(detachedQuery2);
                    listOfQueries.Add(withCountryOnlyDetachedQuery);
                }

                var futureResult = listOfQueries.Select(s => BuildSelectStatement(s, query.Page, query.PageSize)).ToList();

                IEnumerable<TutorCardDto> retVal = futureResult.Select(async s => await s.GetEnumerableAsync(token)).SelectMany(s => s.Result).Distinct(TutorCardDto.UserIdComparer).Take(query.PageSize).ToList();
                return Task.FromResult(retVal);
            }

            private static IFutureEnumerable<TutorCardDto> BuildSelectStatement(IQueryOver<ReadTutor, ReadTutor> futureCourse, int page, int pageSize)
            {
                TutorCardDto tutorCardDtoAlias = null;

                return futureCourse.SelectList(s =>
                        s.Select(x => x.Id).WithAlias(() => tutorCardDtoAlias.UserId)
                            .Select(x => x.Name).WithAlias(() => tutorCardDtoAlias.Name)
                            .Select(x => x.Image).WithAlias(() => tutorCardDtoAlias.Image)
                            .Select(x => x.Courses).WithAlias(() => tutorCardDtoAlias.Courses)
                            .Select(x => x.Subjects).WithAlias(() => tutorCardDtoAlias.Subjects)
                            .Select(x => x.Price).WithAlias(() => tutorCardDtoAlias.Price)
                            .Select(x => x.Rate).WithAlias(() => tutorCardDtoAlias.Rate)
                            .Select(x => x.RateCount).WithAlias(() => tutorCardDtoAlias.ReviewsCount)
                            .Select(x => x.Bio).WithAlias(() => tutorCardDtoAlias.Bio)
                            .Select(x => x.University).WithAlias(() => tutorCardDtoAlias.University)
                            .Select(x => x.Lessons).WithAlias(() => tutorCardDtoAlias.Lessons)
                            .Select(x => x.Country).WithAlias(() => tutorCardDtoAlias.Country))

                    .OrderBy(o => o.OverAllRating).Desc
                    .TransformUsing(Transformers.AliasToBean<TutorCardDto>())
                    .Take(pageSize).Skip(page * pageSize).Future<TutorCardDto>();
            }
        }
    }



}
