using Cloudents.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Cloudents.Query.Email
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Serialize with durable class")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Need fpr deserialize")]

    public class GetUpdatesEmailUsersQuery : IQuery<IEnumerable<UpdateUserEmailDto>>
    {
        public GetUpdatesEmailUsersQuery(DateTime since, int page)
        {
            Since = since;
            Page = page;
        }

        public int Page { get; set; }

        public DateTime Since { get; set; }

        internal sealed class GetUpdatesEmailUsersQueryHandler : IQueryHandler<GetUpdatesEmailUsersQuery, IEnumerable<UpdateUserEmailDto>>
        {
            private readonly IDapperRepository _session;

            public GetUpdatesEmailUsersQueryHandler(IDapperRepository querySession)
            {
                _session = querySession;
            }

            public async Task<IEnumerable<UpdateUserEmailDto>> GetAsync(GetUpdatesEmailUsersQuery query, CancellationToken token)
            {

                const string sql = @"Select distinct u.Name as UserName,
u.Email as ToEmailAddress,
u.Language,
u.Id as UserId 
from sb.[user] u
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
--Temp for now of emails
and u.LastOnline > getUtcDate()-7
order by id
     OFFSET @pageSize * @PageNumber ROWS
                FETCH NEXT @pageSize ROWS ONLY;";

                using (var conn = _session.OpenConnection())
                {
                    return await conn.QueryAsync<UpdateUserEmailDto>(sql, new
                    {
                        query.Since,
                        PageSize = 100,
                        PageNumber = query.Page
                    });
                }
            }
        }
    }
}
