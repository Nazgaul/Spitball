using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query.Stuff;

namespace Cloudents.Query.Users
{
    public class UserAccountQuery : IQuery<UserAccountDto>
    {
        public UserAccountQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }


        internal sealed class UserAccountDataQueryHandler : IQueryHandler<UserAccountQuery, UserAccountDto>
        {
            private readonly IStatelessSession _session;

            public UserAccountDataQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<UserAccountDto> GetAsync(UserAccountQuery query, CancellationToken token)
            {
                //TODO: to nhibernate
                const string sql = @"select u.Id, U.Balance, u.Name, u.ImageName as Image, u.Email, 
                            u.PhoneNumberHash as PhoneNumber,
                            u.Country,
                u.UserType,
                          t.State as IsTutor,
                            coalesce(
                                cast(iif(u.PaymentExists != 0 , 0, null) as bit),
								cast(iif(u.Country != 'IL', 0 , null) as bit),
                                cast(1 as bit)
                            )as NeedPayment
                      from sb.[user] u
                      left join sb.Tutor t
                     on u.Id = t.Id 
                      where U.Id = :Id
                      and (LockoutEnd is null or GetUtcDate() >= LockoutEnd);";

                var userSqlQuery = _session.CreateSQLQuery(sql);
                userSqlQuery.SetInt64("Id", query.Id);
                var userFuture = userSqlQuery.SetResultTransformer(new SbAliasToBeanResultTransformer<UserAccountDto>()).FutureValue<UserAccountDto>();


                const string coursesSql = @"select CourseId as [Name], 
                        c.count as Students,
                        case when c.State = 'Pending' then cast(1 as bit) else cast(null as bit) end as IsPending,
                        uc.CanTeach as IsTeaching
                        from sb.UsersCourses uc
                        join sb.Course c
                        on uc.courseId = c.Name
                        where UserId = :Id
                        order by IsPending desc, Students desc";

                var coursesSqlQuery = _session.CreateSQLQuery(coursesSql);
                coursesSqlQuery.SetInt64("Id", query.Id);
                var coursesFuture = coursesSqlQuery.SetResultTransformer(Transformers.AliasToBean<CourseDto>()).Future<CourseDto>();

                var universityFuture = _session.Query<User>()
                    .Fetch(f => f.University)
                    .Where(w => w.Id == query.Id && w.University != null)
                    .Select(s => new UniversityDto(s.University.Id, s.University.Name, s.University.Country, s.University.Image, s.University.UsersCount))
                    .ToFutureValue();

                var result = await userFuture.GetValueAsync(token);

                result.Courses = await coursesFuture.GetEnumerableAsync(token);
                result.University = await universityFuture.GetValueAsync(token);

                return result;
            }
        }
    }
}