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
        Task SendVerificationCodeAsync(string phoneNumber, CancellationToken token);
        Task<bool> VerifyCodeAsync(string phoneNumber, string code, CancellationToken token);
    }

    public interface IStudyRoomProvider
    {
        Task CreateRoomAsync(string id, Country country, bool needRecord, Uri callBack, StudyRoomTopologyType studyRoomType);
        Task CloseRoomAsync(string id);

        Task<bool> GetRoomAvailableAsync(string id);

        string CreateRoomToken(string roomName, long userId, string name);
    }

    public interface IMailProvider
    {
        Task<bool> ValidateEmailAsync(string email, CancellationToken token);
    }


    public interface ISmsProvider
    {
        Task<string> SendSmsAsync(string message, string phoneNumber, CancellationToken token);
    }


}