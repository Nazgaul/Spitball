using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query.Stuff;
using Cloudents.Core.Enum;

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
                const string sql = @"select u.Id, U.Balance, u.Name, u.FirstName, u.LastName, u.ImageName as Image, u.Email, 
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

                var haveDocsFuture = _session.Query<Document>()
                    .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var haveDocsWithPriceFuture = _session.Query<Document>()
                    .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok && w.Price > 0)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                //TODO: need to return Document with state = archive
                var purchasedDocsFuture = _session.Query<DocumentTransaction>()
                    .Fetch(f => f.User)
                    .Fetch(f => f.Document)
                    .Where(w => w.User.Id == query.Id)
                    .Where(w => w.Document.Status.State == ItemState.Ok)
                    .Where(w => w.Type == TransactionType.Spent)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var purchasedSessionsFuture = _session.Query<StudyRoomSession>()
                    .Fetch(f => f.StudyRoom)
                    .ThenFetch(f => f.Users)
                    .Where(w => w.StudyRoom.Users.Select(s => s.User.Id).Any(a => a == query.Id) && query.Id != w.StudyRoom.Tutor.Id)
                    .Where(w => w.Ended != null)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var haveStudyRoomFuture = _session.Query<StudyRoomUser>()
                    .Fetch(f => f.Room)
                    .Where(w => w.User.Id == query.Id)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var isSoldDocumentFuture = _session.Query<DocumentTransaction>()
                    .Fetch(f => f.User)
                    .Fetch(f => f.Document)
                    .Where(w => w.User.Id == query.Id)
                    .Where(w => w.Type == TransactionType.Earned)
                    .Where(w => w.Document.Status.State == ItemState.Ok)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var isSoldQuestionFuture = _session.Query<QuestionTransaction>()
                    .Fetch(f => f.Answer)
                    .Fetch(f => f.Question)
                    .Where(w => w.Question != null)
                    .Where(w => w.User.Id == query.Id)
                    .Where(w => w.Type == TransactionType.Earned)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var isSoldSessionFuture = _session.Query<StudyRoomSession>()
                    .Fetch(f => f.StudyRoom)
                    .ThenFetch(f => f.Users)
                    .Where(w => w.StudyRoom.Tutor.Id == query.Id && w.Ended != null)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();

                var haveFollowersFuture = _session.Query<Follow>()
                    .Where(w => w.Followed.Id == query.Id)
                    .Select(s => s.Id)
                    .Take(1)
                    .ToFuture();



                var result = await userFuture.GetValueAsync(token);

                result.Courses = await coursesFuture.GetEnumerableAsync(token);
                result.University = await universityFuture.GetValueAsync(token);
                result.HaveDocs = (await haveDocsFuture.GetEnumerableAsync(token)).Any() ? true : false;

                result.HaveDocsWithPrice = (await haveDocsWithPriceFuture.GetEnumerableAsync(token)).Any() ? true : false;

                result.IsPurchased = (await purchasedDocsFuture.GetEnumerableAsync(token)).Any()
                                    || (await purchasedSessionsFuture.GetEnumerableAsync(token)).Any() ? true : false;

                result.HaveStudyRoom = (await haveStudyRoomFuture.GetEnumerableAsync(token)).Any() ? true : false;

                result.IsSold = (await isSoldDocumentFuture.GetEnumerableAsync(token)).Any()
                    || (await isSoldQuestionFuture.GetEnumerableAsync(token)).Any()
                    || (await isSoldSessionFuture.GetEnumerableAsync(token)).Any() ? true : false;

                result.HaveFollowers = (await haveFollowersFuture.GetEnumerableAsync(token)).Any() ? true : false;
                return result;
            }
        }
    }
}