using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IEmailVerification
    {
        Task<bool> VerifyEmailAsync(string email);
        bool VerifyEmail(string email);
    }
}
