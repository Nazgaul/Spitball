using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ISmsProvider
    {
        Task<(string phoneNumber, string country)> ValidateNumberAsync(string phoneNumber, CancellationToken token);
    }

    public interface IVideoProvider
    {
        Task CreateRoomAsync(string id,bool needRecord);
        Task CloseRoomAsync(string id);
        Task<string> ConnectToRoomAsync(string roomName, string name);
    }
}