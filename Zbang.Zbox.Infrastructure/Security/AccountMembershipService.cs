using System;
using System.Web.Security;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class AccountMembershipService : IMembershipService
    {
        public int MinPasswordLength
        {
            get
            {
                return Membership.MinRequiredPasswordLength;
            }
        }

        public LogInStatus ValidateUser(string email, string password, out Guid membershipId)
        {
            membershipId = Guid.Empty;
            Throw.OnNull(email, "email");
            Throw.OnNull(password, "password");

            var userName = Membership.GetUserNameByEmail(email);
            if (string.IsNullOrEmpty(userName))
            {
                return LogInStatus.UserNotExists;

            }
            var user = Membership.GetUser(userName);
            if (user == null)
            {
                throw new NullReferenceException("User");
            }
            if (user.ProviderUserKey == null)
            {
                throw new NullReferenceException("ProviderUserKey");
            }
            membershipId = (Guid)user.ProviderUserKey;
            if (user.IsLockedOut)
            {
                return LogInStatus.LockedOut;
            }

            return Membership.ValidateUser(userName, password) ? LogInStatus.Success : LogInStatus.WrongPassword;
        }


        public MembershipCreateStatus CreateUser(string userName, string password,
            string email, out Guid memberShipUserId)
        {
            memberShipUserId = Guid.NewGuid();
            Throw.OnNull(userName, "userName");
            Throw.OnNull(password, "password");
            Throw.OnNull(email, "email");

            MembershipCreateStatus status;
            var user = Membership.CreateUser(userName, password, email, null, null, true, out status);
            if (user == null)
            {
                return status;
            }

            if (user.ProviderUserKey != null)
            {
                memberShipUserId = (Guid)user.ProviderUserKey;
            }
            return status;
        }

        public bool ChangePassword(Guid membershipId, string oldPassword, string newPassword)
        {
            try
            {
                Throw.OnNull(oldPassword, "oldPassword");
                Throw.OnNull(newPassword, "newPassword");

                MembershipUser currentUser = Membership.GetUser(membershipId);
                if (currentUser == null)
                {
                    TraceLog.WriteInfo("ChangePassword " + membershipId + " cannot find user in membership");
                    return false;
                }
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException ex)
            {
                TraceLog.WriteInfo("ChangePassword " + membershipId);
                TraceLog.WriteError(ex);
                return false;
            }
            catch (MembershipPasswordException ex)
            {
                TraceLog.WriteInfo("ChangePassword " + membershipId);
                TraceLog.WriteError(ex);
                return false;
            }
        }

        //public bool DeleteUser(string email)
        //{
        //    var userName = Membership.GetUserNameByEmail(email);
        //    return Membership.DeleteUser(userName);
        //}

        public bool EmailExists(string email, out Guid membershipId)
        {
            Throw.OnNull(email, "userEmail");

            membershipId = Guid.Empty;
            var userName = Membership.GetUserNameByEmail(email);
            if (string.IsNullOrEmpty(userName))
                return false;
            
            MembershipUser currentUser = Membership.GetUser(userName);

            membershipId = (Guid)currentUser.ProviderUserKey;
            return true;
        }


        public bool ResetPassword(Guid membershipId, string newPassword)
        {
            Throw.OnNull(membershipId, "membershipId");
            Throw.OnNull(newPassword, "newPassword");


            MembershipUser currentUser = Membership.GetUser(membershipId);
            if (currentUser.IsLockedOut)
            {
                currentUser.UnlockUser();
                Membership.UpdateUser(currentUser);
            }
            currentUser.ChangePassword(currentUser.ResetPassword(), newPassword);
            return true;
        }

        public bool ChangeUserEmail(Guid membershipId, string newEmail)
        {
            MembershipUser currentUser = Membership.GetUser(membershipId);
            currentUser.Email = newEmail;

            Membership.UpdateUser(currentUser);
            return true;

        }
    }
}
