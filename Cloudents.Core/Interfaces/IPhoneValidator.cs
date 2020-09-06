using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IPhoneValidator
    {

        Task<(string? phoneNumber, string? country)> ValidateNumberAsync(string phoneNumber, string countryCode, CancellationToken token);
        Task<(string? phoneNumber, string? country)> ValidateNumberAsync(string phoneNumberWithCountryCode, CancellationToken token);
        Task SendVerificationCodeAsync(string phoneNumber, CancellationToken token);
        Task<bool> VerifyCodeAsync(string phoneNumber, string code, CancellationToken token);
    }
}