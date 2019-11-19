using Cloudents.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Services
{
    public interface ISmsSender
    {
        // Task<(string phoneNumber, string country)> ValidateNumberAsync(string phoneNumber, string countryCode, CancellationToken token);
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