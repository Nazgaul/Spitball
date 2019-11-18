using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class TutorListByCourseQuery : IQuery<IEnumerable<TutorCardDto>>
    {
        public TutorListByCourseQuery(string courseId, long userId, string country, int count, int page = 0)
        {
            CourseId = courseId;
            UserId = userId;
            Country = country;
            Count = count;
            Page = page;
        }


        /// <summary>
        /// The course name we want to get tutors
        /// </summary>
        private string CourseId { get; }
        /// <summary>
        /// Eliminate the current user from the result
        /// </summary>
        private long UserId { get; }

        private string Country { get; }

        private int Count { get; }
        public int Page { get; set; }

        internal sealed class TutorListByCourseQueryHandler : IQueryHandler<TutorListByCourseQuery, IEnumerable<TutorCardDto>>
        {

            private readonly IStatelessSession _session;

            public TutorListByCourseQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<TutorCardDto>> GetAsync(TutorListByCourseQuery query, CancellationToken token)
            {
                //TODO maybe we can fix this query
                Core.Entities.ReadTutor tutorAlias = null;
                UserCourse userCourseAlias = null;
                Course courseAlias = null;
                TutorCardDto tutorCardDtoAlias = null;
                var relevantTutorByCourse = QueryOver.Of(() => tutorAlias)
                    .JoinEntityAlias(() => userCourseAlias,
                        () => userCourseAlias.User.Id == tutorAlias.Id)
                    .Where(() => userCourseAlias.CanTeach)
                    .And(() => userCourseAlias.Course.Id == query.CourseId)
                    .Select(s => s.Id)
                    .Take(query.Count);


                var relevantTutorBySubject = QueryOver.Of(() => tutorAlias)
                    .JoinEntityAlias(() => userCourseAlias,
                        () => userCourseAlias.User.Id == tutorAlias.Id)
                    .JoinAlias(() => userCourseAlias.Course, () => courseAlias)
                    .Where(() => userCourseAlias.CanTeach)
                    .WithSubquery.WhereProperty(() => courseAlias.Subject.Id).Eq(
                        QueryOver.Of<Course>().Where(w => w.Id == query.CourseId)
                            .Select(s => s.Subject.Id))
                    .Select(s => s.Id)
                    .Take(query.Count);


                var futureCourse = _session.QueryOver<ReadTutor>()
                     .WithSubquery.WhereProperty(w => w.Id).In(relevantTutorByCourse)
                     .Where(w => w.Id != query.UserId)
                     .And(w => w.Country == query.Country)
                     .SelectList(s =>
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
                             .Select(x => x.Country).WithAlias(() => tutorCardDtoAlias.Country)
                             .Select(x => x.Lessons).WithAlias(() => tutorCardDtoAlias.Lessons))

                     .OrderBy(o => o.OverAllRating).Desc

                     .TransformUsing(Transformers.AliasToBean<TutorCardDto>())
                     .Take(query.Count).Future<TutorCardDto>();

                var futureCourse2 = _session.QueryOver<ReadTutor>()
                    .WithSubquery.WhereProperty(w => w.Id).In(relevantTutorBySubject)
                    .Where(w => w.Id != query.UserId)
                    .And(w => w.Country == query.Country)
                    .SelectList(s =>
                        s.Select(x => x.Id).WithAlias(() => tutorCardDtoAlias.UserId)
                            .Select(x => x.Name).WithAlias(() => tutorCardDtoAlias.Name)
                            .Select(x => x.Image).WithAlias(() => tutorCardDtoAlias.Image)
                            .Select(x => x.Courses).WithAlias(() => tutorCardDtoAlias.Courses)
                            .Select(x => x.Subjects).WithAlias(() => tutorCardDtoAlias.Subjects)
                            .Select(x => x.Price).WithAlias(() => tutorCardDtoAlias.Price)
                            .Select(x => x.Rate).WithAlias(() => tutorCardDtoAlias.Rate)
                            .Select(x => x.RateCount).WithAlias(() => tutorCardDtoAlias.ReviewsCount)
                            .Select(x => x.Bio).WithAlias(() => tutorCardDtoAlias.Bio)
                            .Select(x => x.Country).WithAlias(() => tutorCardDtoAlias.Country)
                            .Select(x => x.University).WithAlias(() => tutorCardDtoAlias.University)
                            .Select(x => x.Lessons).WithAlias(() => tutorCardDtoAlias.Lessons))

                    .OrderBy(o => o.OverAllRating).Desc

                    .TransformUsing(Transformers.AliasToBean<TutorCardDto>())
                    .Take(query.Count).Future<TutorCardDto>();
                var tutors = await futureCourse.GetEnumerableAsync(token);
                var tutors2 = await futureCourse2.GetEnumerableAsync(token);

                return tutors.Union(tutors2).Skip(query.Page * query.Count).Take(query.Count)
                    .Distinct(TutorCardDto.UserIdComparer).ToList();
            }
        }
    }
}