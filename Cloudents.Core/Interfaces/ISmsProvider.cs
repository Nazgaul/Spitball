using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ISmsProvider
    {

        Task<(string phoneNumber, string country)> ValidateNumberAsync(string phoneNumber, string countryCode, CancellationToken token);
    }

    public interface IVideoProvider
    {
        Task CreateRoomAsync(string id,bool needRecord, Uri callbackUri);
        Task CloseRoomAsync(string id);

        Task<bool> GetRoomAvailableAsync(string id);
        Task<string> ConnectToRoomAsync(string roomName, string name);
    }
}