using Cloudents.Core.DTOs.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminConversationDetailsQuery: IQuery<IEnumerable<ConversationDetailsDto>>
    {
        public AdminConversationDetailsQuery(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }

        internal sealed class AdminConversationDetailsQueryHandler : IQueryHandler<AdminConversationDetailsQuery, IEnumerable<ConversationDetailsDto>>
        {
            private readonly DapperRepository _dapper;


            public AdminConversationDetailsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }


            public async Task<IEnumerable<ConversationDetailsDto>> GetAsync(AdminConversationDetailsQuery query, CancellationToken token)
            {
                const string sql = @"select distinct u.Name as UserName, u.Email, u.PhoneNumberHash as PhoneNumber
                                from sb.ChatMessage cm
                                join sb.[User] u
	                                on cm.UserId = u.Id
                                where cm.ChatRoomId = @Id";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<ConversationDetailsDto>(sql, new { query .Id });
                    return res;
                }
            }
        }
    }
}
