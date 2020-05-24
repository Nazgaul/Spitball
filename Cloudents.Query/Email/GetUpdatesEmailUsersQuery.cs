using Cloudents.Core.DTOs.Email;
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

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

                const string sql = @"
Select distinct u.Name as UserName,
u.Email as ToEmailAddress,
u.Language,
u.Id as UserId 
from sb.[user] u
join sb.UsersRelationship uc on u.id = uc.FollowerId
join
 (Select d.UserId  from sb.Document  d
where state = 'Ok'
and d.CreationTime > @Since
union 
Select q.UserId   from sb.question  q
where state = 'Ok'
and q.Created > @Since
) t
on  t.UserId  = uc.UserId
where u.SbCountry <> @Country
--and u.EmailConfirmed = 1
--Temp for now of emails
and u.LastOnline > getUtcDate()-90
order by u.id
     OFFSET @pageSize * @PageNumber ROWS
                FETCH NEXT @pageSize ROWS ONLY;";

                using var conn = _session.OpenConnection();
                return await conn.QueryAsync<UpdateUserEmailDto>(sql, new
                {
                    query.Since,
                    Country = Country.India,
                    PageSize = 100,
                    PageNumber = query.Page
                });
            }
        }
    }
}
