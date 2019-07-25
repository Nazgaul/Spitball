using Cloudents.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Cloudents.Query.Email
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global" ,Justification = "Serialize with durable class")]
    public class GetUpdatesEmailUsersQuery : IQuery<IEnumerable<UpdateUserEmailDto>>
    {
        public GetUpdatesEmailUsersQuery(DateTime since, int page)
        {
            Since = since;
            Page = page;
        }
        
        public int Page { get;   set; }
        public DateTime Since { get;  set; }

        internal sealed class GetUpdatesEmailUsersQueryHandler : IQueryHandler<GetUpdatesEmailUsersQuery, IEnumerable<UpdateUserEmailDto>>
        {
            private readonly DapperRepository _session;

            public GetUpdatesEmailUsersQueryHandler(DapperRepository querySession)
            {
                _session = querySession;
            }

            public async Task<IEnumerable<UpdateUserEmailDto>> GetAsync(GetUpdatesEmailUsersQuery query, CancellationToken token)
            {

                const string sql = @"Select distinct u.Name as UserName,u.Email as ToEmailAddress,u.Language,u.Id as UserId from sb.[user] u
join sb.UsersCourses uc on u.id = uc.UserId
join
 (Select d.UniversityId as UniversityId,d.CourseName  from sb.Document  d
where state = 'Ok'
and d.CreationTime > @Since
union 
Select q.UniversityId as UniversityId,q.CourseId as CourseName  from sb.question  q
where state = 'Ok'
and q.Created > @Since
) t
on u.UniversityId2 = t.UniversityId and t.CourseName  = uc.CourseId
where u.EmailConfirmed = 1
order by id
     OFFSET @pageSize * @PageNumber ROWS
                FETCH NEXT @pageSize ROWS ONLY;";

                using (var conn = _session.OpenConnection())
                {
                  return  await conn.QueryAsync<UpdateUserEmailDto>(sql, new
                    {
                        Since = query.Since,
                        PageSize = 100,
                        PageNumber = query.Page
                    });
                }

 //               Question questionAlias = null;
 //               Document documentAlias = null;
 //               UserCourse userCourseAlias = null;
 //               User userAlias = null;

 //               UpdateEmailDto updateEmailAlias = null;


 //               var queryQuestion = QueryOver.Of(() => questionAlias)
 //                   .JoinEntityAlias(() => userCourseAlias, () => questionAlias.Course.Id == userCourseAlias.Course.Id)
 //                   .Where(w => w.Created > query.Since)
 //                   .And(x => x.Status.State == ItemState.Ok)
 //                   .Select(x => userCourseAlias.User.Id);
 //               //.Select(() => userCourseAlias.User.Id);


 //               var queryDocument = QueryOver.Of(() => documentAlias)
 //                   .JoinEntityAlias(() => userCourseAlias, () => documentAlias.Course.Id == userCourseAlias.Course.Id)
 //                   .Where(w => w.TimeStamp.CreationTime > query.Since)
 //                   .And(x => x.Status.State == ItemState.Ok)
 //                   .Select(x => userCourseAlias.User.Id);
 //               //.Select(s => s.User.Id);


 //               return await
 //                    _session.QueryOver(() => userAlias)
 //                        .Where(w => w.LockoutEnd < DateTimeOffset.UtcNow)

 //                        .And(Restrictions.Disjunction()
 //                            .Add(Subqueries.WhereProperty(() => userAlias.Id).In(queryQuestion))
 //                            .Add(Subqueries.WhereProperty(() => userAlias.Id).In(queryDocument))

 //)
 //                    .SelectList(sl =>
 //                    {
 //                        sl.Select(() => userAlias.Id).WithAlias(() => updateEmailAlias.UserId);
 //                        sl.Select(() => userAlias.Email).WithAlias(() => updateEmailAlias.ToEmailAddress);
 //                        sl.Select(() => userAlias.Language).WithAlias(() => updateEmailAlias.Language);
 //                        sl.Select(() => userAlias.Name).WithAlias(() => updateEmailAlias.UserName);
 //                        return sl;
 //                    }).TransformUsing(Transformers.AliasToBean<UpdateEmailDto>())
 //                        .OrderBy(x => x.Id).Asc
 //                    .Take(100).Skip(100 * query.Page).ListAsync<UpdateEmailDto>(token);





            }
        }
    }
}
