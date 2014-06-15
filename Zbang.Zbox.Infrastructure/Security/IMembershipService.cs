using System;
using System.Web.Security;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Security
{
    public interface IMembershipService
    {
        int MinPasswordLength { get; }
        LogInStatus ValidateUser(string email, string password, out Guid membershipId);
        //bool ValidateUser(string email, string password, out Guid membershipId);
        MembershipCreateStatus CreateUser(string userName, string password, string email, out Guid memberShipUserId);
        bool ChangePassword(Guid membershipId, string oldPassword, string newPassword);
        
        bool ChangeUserEmail(Guid membershipId, string newEmail);

        //bool DeleteUser(string email);

        bool EmailExists(string email, out Guid membershipId);
        bool ResetPassword(Guid membershipId, string newPassword);
    }
}
