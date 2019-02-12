using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Web.Services
{
    public interface ISmsSender
    {
      //  Task SendSmsAsync(string phoneNumber, string code, CancellationToken token);
        Task<string> ValidateNumberAsync(string phoneNumber, CancellationToken token);
        /// <summary>
        /// User signin manager with TwoFactorSignInAsync
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SendSmsAsync(RegularUser user, CancellationToken token);

        Task SendPhoneAsync(RegularUser user, CancellationToken token);
    }
}