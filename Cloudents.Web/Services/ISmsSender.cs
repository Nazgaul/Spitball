using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Web.Services
{
    public interface ISmsSender
    {
      //  Task SendSmsAsync(string phoneNumber, string code, CancellationToken token);
        Task<(string phoneNumber, string country)> ValidateNumberAsync(string phoneNumber, CancellationToken token);
        /// <summary>
        /// User signin manager with TwoFactorSignInAsync
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SendSmsAsync(User user, CancellationToken token);

        Task SendPhoneAsync(User user, CancellationToken token);
    }
}