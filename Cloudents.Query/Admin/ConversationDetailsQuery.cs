using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class ConversationDetailsQuery : IQueryAdmin<IEnumerable<ConversationDetailsDto>>
    {
        public ConversationDetailsQuery(string id, string country)
        {
            Id = id;
            Country = country;
        }

        private string Id { get; }
        public string Country { get; }

        internal sealed class ConversationDetailsQueryHandler : IQueryHandler<ConversationDetailsQuery, IEnumerable<ConversationDetailsDto>>
        {
            private readonly IDapperRepository _dapper;

            public ConversationDetailsQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }


            public async Task<IEnumerable<ConversationDetailsDto>> GetAsync(ConversationDetailsQuery query, CancellationToken token)
            {
                var sql = @"with cte as (
select u.Id as UserId, cr.Id as ChatRoomId, u.Name as UserName, u.Email, u.PhoneNumberHash as PhoneNumber, u.ImageName as Image,
case when u.Id = (select top 1 UserId from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by cm.CreationTime) 
	then 1
else 0 end as Student,
u.Country
from sb.ChatRoom cr
join sb.ChatUser cu
	on cu.ChatRoomId = cr.Id
join sb.[User] u
	on u.Id = cu.UserId 
where cr.identifier = @Id
)
select * from cte";
                if (!string.IsNullOrEmpty(query.Country))
                {
                    sql += @" where Student = 1 and @Country = (select Country from cte where Student = 0) or Student = 0 
                                and Country = @Country";

                }
                sql += @" order by case when UserId = (select top 1 UserId from sb.ChatMessage cm where cm.ChatRoomId = ChatRoomId order by cm.CreationTime) then 1
                            else 0 end";
                using var connection = _dapper.OpenConnection();
                var res = await connection.QueryAsync<ConversationDetailsDto>(sql, new { query.Id, query.Country });
                return res;
            }
        }
    }
}
