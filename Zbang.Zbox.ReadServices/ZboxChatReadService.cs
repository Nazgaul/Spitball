using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxChatReadService : IZboxChatReadService
    {
        private readonly IDocumentDbRepository<ChatRoom> m_ChatRoomRepository;

        public ZboxChatReadService(IDocumentDbRepository<ChatRoom> chatRoomRepository)
        {
            m_ChatRoomRepository = chatRoomRepository;
        }

        public async Task<IEnumerable<ViewModel.Dto.UserDtos.UserWithStatusDto>> GetUserWithConversationAsync(QueryBase query)
        {
            var documentResult = await m_ChatRoomRepository.GetItemsAsync($"SELECT c.users FROM c join user in c.users where user.id = {query.UserId}");
            var userResult = documentResult.Select(s => s.Users).SelectMany(zz => zz).Where(w => w.Id != 1).Select(s => s.Id);
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<ViewModel.Dto.UserDtos.UserWithStatusDto>(ViewModel.SqlQueries.Chat.MessageBoard,
                     new
                     {
                         UserIds = userResult

                     });
            }
        }
    }
}
