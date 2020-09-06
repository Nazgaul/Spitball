using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface IChatRoomRepository : IRepository<ChatRoom>
    {
        Task<ChatRoom> GetOrAddChatRoomAsync(IEnumerable<long> userIds, CancellationToken token);

        Task<ChatRoom?> GetChatRoomAsync(string identifier, CancellationToken token);
        Task UpdateNonDayOldConversationToActiveAsync(CancellationToken token);
        Task<ChatRoom> GetOrAddChatRoomAsync(IEnumerable<long> userIds, Tutor tutor, CancellationToken token);
    }
}