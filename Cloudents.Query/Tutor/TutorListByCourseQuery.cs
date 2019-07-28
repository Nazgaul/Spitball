using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Dapper;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace Cloudents.Query.Tutor
{
    public class TutorListByCourseQuery : IQuery<IEnumerable<TutorCardDto>>
    {
        public TutorListByCourseQuery(string courseId, long userId, int count)
        {
            CourseId = courseId;
            UserId = userId;
            Count = count;
        }


        /// <summary>
        /// The course name we want to get tutors
        /// </summary>
        private string CourseId { get; }
        /// <summary>
        /// Eliminate the current user from the result
        /// </summary>
        private long UserId { get; }

        private int Count { get; }

        internal sealed class TutorListByCourseQueryHandler : IQueryHandler<TutorListByCourseQuery, IEnumerable<TutorCardDto>>
        {
            private readonly IStatelessSession _session;

            public TutorListByCourseQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<TutorCardDto>> GetAsync(TutorListByCourseQuery query, CancellationToken token)
            {
                Core.Entities.Tutor tutorAlias = null;
                UserCourse userCourseAlias = null;
                Course courseAlias = null;
                TutorCardDto tutorCardDtoAlias = null;
                var detachedQuery = QueryOver.Of(() => tutorAlias)
                    .JoinEntityAlias(() => userCourseAlias, () => userCourseAlias.User.Id == tutorAlias.Id)
                    .Where(() => userCourseAlias.CanTeach)
                    .And(() => tutorAlias.State == ItemState.Ok)
                    .And(() => userCourseAlias.Course.Id == query.CourseId)
                    .Select(s => s.Id)
                    .Take(query.Count);


                var detachedQuery2 = QueryOver.Of(() => tutorAlias)
                    .JoinEntityAlias(() => userCourseAlias, () => userCourseAlias.User.Id == tutorAlias.Id)
                    .JoinAlias(() => userCourseAlias.Course,() => courseAlias)
                    .Where(() => userCourseAlias.CanTeach)
                    .And(() => tutorAlias.State == ItemState.Ok)
                    .WithSubquery.WhereProperty(() => courseAlias.Subject.Id).Eq(
                        QueryOver.Of<Course>().Where(w=>w.Id == query.CourseId).Select(s=>s.Subject.Id))
                    .Select(s => s.Id)
                    .Take(query.Count);


                var futureCourse =  _session.QueryOver<ViewTutor>()
                     .WithSubquery.WhereProperty(w => w.Id).In(detachedQuery)
                     .Where(w => w.Id != query.UserId)
                   
                     .SelectList(s =>
                         s.Select(x => x.Id).WithAlias(() => tutorCardDtoAlias.UserId)
                             
                             .Select(x => x.Name).WithAlias(() => tutorCardDtoAlias.Name)
                             .Select(x => x.Image).WithAlias(() => tutorCardDtoAlias.Image)
                             .Select(x => x.Courses).WithAlias(() => tutorCardDtoAlias.Courses)
                             .Select(x => x.CourseCount).WithAlias(() => tutorCardDtoAlias.CourseCount)
                             .Select(x => x.Subjects).WithAlias(() => tutorCardDtoAlias.Subjects)
                             .Select(x => x.Price).WithAlias(() => tutorCardDtoAlias.Price)
                             .Select(x => x.Rate).WithAlias(() => tutorCardDtoAlias.Rate)
                             .Select(x => x.SumRate).WithAlias(() => tutorCardDtoAlias.ReviewsCount)
                             .Select(x => x.Bio).WithAlias(() => tutorCardDtoAlias.Bio)
                             .Select(x => x.University).WithAlias(() => tutorCardDtoAlias.University)
                             .Select(x => x.Lessons).WithAlias(() => tutorCardDtoAlias.Lessons))
                    
                     .OrderBy(o => o.SumRate).Desc
                     
                     .TransformUsing(Transformers.AliasToBean<TutorCardDto>())
                     .Take(query.Count).Future<TutorCardDto>();

                var futureCourse2 = _session.QueryOver<ViewTutor>()
                    .WithSubquery.WhereProperty(w => w.Id).In(detachedQuery2)
                    .Where(w => w.Id != query.UserId)

                    .SelectList(s =>
                        s.Select(x => x.Id).WithAlias(() => tutorCardDtoAlias.UserId)
                            .Select(x => x.Name).WithAlias(() => tutorCardDtoAlias.Name)
                            .Select(x => x.Image).WithAlias(() => tutorCardDtoAlias.Image)
                            .Select(x => x.Courses).WithAlias(() => tutorCardDtoAlias.Courses)
                            .Select(x => x.CourseCount).WithAlias(() => tutorCardDtoAlias.CourseCount)
                            .Select(x => x.Subjects).WithAlias(() => tutorCardDtoAlias.Subjects)
                            .Select(x => x.Price).WithAlias(() => tutorCardDtoAlias.Price)
                            .Select(x => x.Rate).WithAlias(() => tutorCardDtoAlias.Rate)
                            .Select(x => x.SumRate).WithAlias(() => tutorCardDtoAlias.ReviewsCount)
                            .Select(x => x.Bio).WithAlias(() => tutorCardDtoAlias.Bio)
                            .Select(x => x.University).WithAlias(() => tutorCardDtoAlias.University)
                            .Select(x => x.Lessons).WithAlias(() => tutorCardDtoAlias.Lessons))

                    .OrderBy(o => o.SumRate).Desc

                    .TransformUsing(Transformers.AliasToBean<TutorCardDto>())
                    .Take(query.Count).Future<TutorCardDto>();
                var tutors = await futureCourse.GetEnumerableAsync(token);
                var tutors2 = await futureCourse2.GetEnumerableAsync(token);

                return tutors.Union(tutors2).Take(query.Count).Distinct();




                //                const string sql = @"select *  from (select 2 as position, U.Id as UserId, U.Name, U.Image,
                //(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
                //from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
                //T.Price, 
                //T.Bio,
                //	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate,
                //                            (select count(1) from sb.TutorReview where TutorId = T.Id) as ReviewsCount
                //                        from sb.[user] U
                //                        join sb.Tutor T
                //	                        on U.Id = T.Id
                //						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
                //						and uc.CourseId = @CourseId
                //                        and T.State = 'Ok'

                //union all
                //select 1 as position, U.Id as UserId, U.Name, U.Image, 
                //(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
                //from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
                //T.Price, 
                //T.Bio,
                //	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate,
                //                            (select count(1) from sb.TutorReview where TutorId = T.Id) as ReviewsCount
                //                        from sb.[user] U
                //                        join sb.Tutor T
                //	                        on U.Id = T.Id
                //						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
                //						join sb.Course c on uc.CourseId = c.Name
                //						and c.SubjectId = (Select subjectId from sb.Course where Name = @CourseId)
                //                        and T.State = 'Ok'
                //) t
                //where t.UserId <> @UserId
                //order by position desc, Rate desc
                //OFFSET 0 ROWS
                //FETCH NEXT @Count ROWS ONLY;";
                //                using (var conn = _dapperRepository.OpenConnection())
                //                {
                //                    var retVal = await conn.QueryAsync<TutorCardDto>(sql, new
                //                    {
                //                        query.CourseId,
                //                        query.UserId,
                //                        query.Count
                //                    });

                //                    return retVal.Distinct(TutorCardDto.UserIdComparer);
                //                }
            }
        }
    }
}