using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure
{
    public interface IEmailVerification
    {
        Task<bool> VerifyEmailAsync(string email);
        bool VerifyEmail(string email);
    }
}
