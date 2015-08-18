using System;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Security
{

    public interface IAccountService
    {
        Task<bool> ChangePassword(Guid accountId, string oldPassword, string newPassword);
        Task<Guid?> CreateUser(string email, string password);
        Task<bool> ChangeEmail(Guid accountId, string newEmail);
    }
}
