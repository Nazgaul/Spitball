using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminConversationDetailsQuery: IQuery<IEnumerable<ConversationDetailsDto>>
    {
        public AdminConversationDetailsQuery(string id)
        {
            Id = id;
        }

        private string Id { get;  }

        internal sealed class AdminConversationDetailsQueryHandler : IQueryHandler<AdminConversationDetailsQuery, IEnumerable<ConversationDetailsDto>>
        {
            private readonly DapperRepository _dapper;


            public AdminConversationDetailsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }


            public async Task<IEnumerable<ConversationDetailsDto>> GetAsync(AdminConversationDetailsQuery query, CancellationToken token)
            {
                const string sql = @"select u.Name as UserName, u.Email, u.PhoneNumberHash as PhoneNumber,
case when u.Id = (select top 1 UserId from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by cm.CreationTime) then 1
else 0 end as Student
from sb.ChatRoom cr
join sb.ChatUser cu on cr.id = cu.ChatRoomId
join sb.[User] u on cu.UserId = u.Id
    where cr.identifier = @Id
    order by case when u.Id = (select top 1 UserId from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by cm.CreationTime) then 1
else 0 end";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<ConversationDetailsDto>(sql, new { query.Id });
                    return res;
                }
            }
        }
    }
}
