using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Cloudents.Query.Tutor
{
    public class TutorListQuery : IQuery<IEnumerable<TutorCardDto>>
    {
        public TutorListQuery(long userId, string country)
        {
            UserId = userId;
            Country = country;
        }


        private long UserId { get; }
        private string Country { get; }

        internal sealed class TutorListQueryHandler : IQueryHandler<TutorListQuery, IEnumerable<TutorCardDto>>
        {
            private readonly IStatelessSession _session;

            public TutorListQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            //TODO: review query 
            public async Task<IEnumerable<TutorCardDto>> GetAsync(TutorListQuery query, CancellationToken token)
            {
                User userAlias = null;
                ViewTutor viewTutorAlias = null;
                Core.Entities.Tutor tutorAlias = null;
                UserCourse userCourseAlias = null;
                Course courseAlias = null;


                var listOfQueries = new List<IQueryOver<ViewTutor, ViewTutor>>();
                IQueryOver<ViewTutor, ViewTutor> futureCourse = _session.QueryOver(() => viewTutorAlias);

                listOfQueries.Add(futureCourse);
                if (!string.IsNullOrEmpty(query.Country))
                {
                    futureCourse.JoinEntityAlias(() => userAlias, () => viewTutorAlias.Id == userAlias.Id);
                    futureCourse.Where(() => userAlias.Country == query.Country);
                }



                if (query.UserId > 0)
                {
                    futureCourse.Where(w => w.Id != query.UserId);
                    
                    var detachedQuery = QueryOver.Of(() => tutorAlias)
                        .JoinEntityAlias(() => userCourseAlias, () => userCourseAlias.User.Id == tutorAlias.Id)
                        .Where(() => userCourseAlias.CanTeach)
                        .And(() => tutorAlias.State == ItemState.Ok)
                        .WithSubquery.WhereProperty(() => userCourseAlias.Course.Id).In(
                            QueryOver.Of<UserCourse>()
                                .Where(w => w.User.Id == query.UserId).Select(s => s.Course.Id))
                        .Select(s => s.Id);


                    var detachedQuery2 = QueryOver.Of(() => tutorAlias)
                        .JoinEntityAlias(() => userCourseAlias, () => userCourseAlias.User.Id == tutorAlias.Id)
                        .JoinAlias(() => userCourseAlias.Course, () => courseAlias)
                        .Where(() => userCourseAlias.CanTeach)
                        .And(() => tutorAlias.State == ItemState.Ok)
                        .WithSubquery.WhereProperty(() => courseAlias.Subject.Id).In(
                            QueryOver.Of<Course>()
                                .JoinQueryOver(x => x.Users)
                                .Where(w => w.User.Id == query.UserId).Select(s => s.Subject.Id))
                        .Select(s => s.Id);
                    var futureCourse2 = futureCourse.Clone();

                    listOfQueries.Add(futureCourse2);



                    futureCourse.WithSubquery.WhereProperty(w => w.Id).In(detachedQuery);
                    futureCourse2.WithSubquery.WhereProperty(w => w.Id).In(detachedQuery2);
                }

                var futureResult = listOfQueries.Select(BuildSelectStatement).ToList();


                return futureResult.Select(async s => await s.GetEnumerableAsync(token)).SelectMany(s=>s.Result).Distinct().Take(20).ToList();
                //foreach (var future in futureResult)
                //{
                //    future.GetEnumerableAsync()
                //}
                //foreach (var listOfQuery in listOfQueries)
                //{
                //     BuildSelectStatement(listOfQuery);
                //}
                //var result = BuildSelectStatement(futureCourse);

                //return await result.GetEnumerableAsync(token);


                //var detachedQuery = QueryOver.Of(() => tutorAlias)
                //    .JoinEntityAlias(() => userCourseAlias, () => userCourseAlias.User.Id == tutorAlias.Id)
                //    .Where(() => userCourseAlias.CanTeach)
                //    .And(() => tutorAlias.State == ItemState.Ok)
                //    //.And(() => userCourseAlias.Course.Id == query.CourseId)
                //    .Select(s => s.Id)
                //    .Take(10);


                //var detachedQuery2 = QueryOver.Of(() => tutorAlias)
                //    .JoinEntityAlias(() => userCourseAlias, () => userCourseAlias.User.Id == tutorAlias.Id)
                //    .JoinAlias(() => userCourseAlias.Course, () => courseAlias)
                //    .Where(() => userCourseAlias.CanTeach)
                //    .And(() => tutorAlias.State == ItemState.Ok)
                //    .WithSubquery.WhereProperty(() => courseAlias.Subject.Id).Eq(
                //        QueryOver.Of<Course>().Where(w => w.Id == query.CourseId).Select(s => s.Subject.Id))
                //    .Select(s => s.Id)
                //    .Take(10);


                //var futureCourse = _session.QueryOver<ViewTutor>()
                //     .WithSubquery.WhereProperty(w => w.Id).In(detachedQuery)
                //     .Where(w => w.Id != query.UserId)

                //     .SelectList(s =>
                //         s.Select(x => x.Id).WithAlias(() => tutorCardDtoAlias.UserId)
                //             .Select(x => x.Name).WithAlias(() => tutorCardDtoAlias.Name)
                //             .Select(x => x.Image).WithAlias(() => tutorCardDtoAlias.Image)
                //             .Select(x => x.Courses).WithAlias(() => tutorCardDtoAlias.Courses)
                //             .Select(x => x.CourseCount).WithAlias(() => tutorCardDtoAlias.CourseCount)
                //             .Select(x => x.Subjects).WithAlias(() => tutorCardDtoAlias.Subjects)
                //             .Select(x => x.Price).WithAlias(() => tutorCardDtoAlias.Price)
                //             .Select(x => x.Rate).WithAlias(() => tutorCardDtoAlias.Rate)
                //             .Select(x => x.SumRate).WithAlias(() => tutorCardDtoAlias.ReviewsCount)
                //             .Select(x => x.Bio).WithAlias(() => tutorCardDtoAlias.Bio)
                //             .Select(x => x.University).WithAlias(() => tutorCardDtoAlias.University)
                //             .Select(x => x.Lessons).WithAlias(() => tutorCardDtoAlias.Lessons))

                //     .OrderBy(o => o.SumRate).Desc

                //     .TransformUsing(Transformers.AliasToBean<TutorCardDto>())
                //     .Take(query.Count).Future<TutorCardDto>();

                //var futureCourse2 = _session.QueryOver<ViewTutor>()
                //    .WithSubquery.WhereProperty(w => w.Id).In(detachedQuery2)
                //    .Where(w => w.Id != query.UserId)

                //    .SelectList(s =>
                //        s.Select(x => x.Id).WithAlias(() => tutorCardDtoAlias.UserId)
                //            .Select(x => x.Name).WithAlias(() => tutorCardDtoAlias.Name)
                //            .Select(x => x.Image).WithAlias(() => tutorCardDtoAlias.Image)
                //            .Select(x => x.Courses).WithAlias(() => tutorCardDtoAlias.Courses)
                //            .Select(x => x.CourseCount).WithAlias(() => tutorCardDtoAlias.CourseCount)
                //            .Select(x => x.Subjects).WithAlias(() => tutorCardDtoAlias.Subjects)
                //            .Select(x => x.Price).WithAlias(() => tutorCardDtoAlias.Price)
                //            .Select(x => x.Rate).WithAlias(() => tutorCardDtoAlias.Rate)
                //            .Select(x => x.SumRate).WithAlias(() => tutorCardDtoAlias.ReviewsCount)
                //            .Select(x => x.Bio).WithAlias(() => tutorCardDtoAlias.Bio)
                //            .Select(x => x.University).WithAlias(() => tutorCardDtoAlias.University)
                //            .Select(x => x.Lessons).WithAlias(() => tutorCardDtoAlias.Lessons))

                //    .OrderBy(o => o.SumRate).Desc

                //    .TransformUsing(Transformers.AliasToBean<TutorCardDto>())
                //    .Take(query.Count).Future<TutorCardDto>();
                //var tutors = await futureCourse.GetEnumerableAsync(token);
                //var tutors2 = await futureCourse2.GetEnumerableAsync(token);

                //return tutors.Union(tutors2).Take(query.Count).Distinct();
                //                const string sql = @"select distinct U.Id as UserId, U.Name, U.Image, 
                //(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
                //from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
                //T.Price,
                //T.Bio,
                //case when uc.CourseId in (select CourseId from sb.UsersCourses where UserId = @UserId or @UserId = 0) then 2
                //when c.SubjectId in (Select subjectId  from sb.UsersCourses where UserId = @UserId or @UserId = 0) then 1
                //else 0 end as t,
                //x.*

                //                        from sb.[user] U
                //                        join sb.Tutor T
                //	                        on U.Id = T.Id
                //						left join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
                //						and  uc.CourseId in (select CourseId from sb.UsersCourses where UserId = @UserId or @UserId = 0)
                //						left join sb.Course c on uc.CourseId = c.Name

                //cross apply (select avg(Rate) as Rate, count(1) as ReviewsCount from sb.TutorReview where TutorId = T.Id) as x
                //where (t.Id <> @UserId or @UserId = 0) 
                //                        and T.State = 'Ok'
                //                        and (U.Country = @Country or @Country is null) 
                //order by t desc, Rate desc
                //OFFSET 0 ROWS
                //FETCH NEXT 20 ROWS ONLY;";


                //                using (var conn = _dapperRepository.OpenConnection())
                //                {
                //                    var retVal = await conn.QueryAsync<TutorCardDto>(sql, new { query.UserId, query.Country });

                //                    return retVal.Take(20);

                //                }
            }

            private static IFutureEnumerable<TutorCardDto> BuildSelectStatement(IQueryOver<ViewTutor, ViewTutor> futureCourse)
            {
                TutorCardDto tutorCardDtoAlias = null;

                return futureCourse.SelectList(s =>
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
                    .Take(20).Future<TutorCardDto>();
            }
        }
    }



}
