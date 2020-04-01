using Cloudents.Core.Enum;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface IPhoneValidator
    {

        Task<(string? phoneNumber, string? country)> ValidateNumberAsync(string phoneNumber, string countryCode, CancellationToken token);
    }

    public interface IVideoProvider
    {
        Task CreateRoomAsync(string id, Country country, bool needRecord, Uri callBack, StudyRoomType studyRoomType);
        Task CloseRoomAsync(string id);

        Task<bool> GetRoomAvailableAsync(string id);
        //Task<string?> ConnectToRoomAsync(string roomName, long userId);

        string CreateRoomToken(string roomName, long userId);
    }

    public interface IMailProvider
    {
        Task<bool> ValidateEmailAsync(string email, CancellationToken token);
    }


    public interface ISmsProvider
    {
        Task SendSmsAsync(string message, string phoneNumber, CancellationToken token);
    }


}