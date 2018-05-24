using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Chat;

namespace Cloudents.Core.Interfaces
{
    public interface IChat
    {
        Task CreateOrUpdateUserAsync(long id, User user, CancellationToken token);
    }
}