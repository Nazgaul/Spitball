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
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

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
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

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
                if (string.IsNullOrEmpty(oldPassword))
                {
                    throw new ArgumentNullException("oldPassword");
                }
                if (string.IsNullOrEmpty(newPassword))
                {
                    throw new ArgumentNullException("newPassword");
                }

                MembershipUser currentUser = Membership.GetUser(membershipId);
                if (currentUser != null) return currentUser.ChangePassword(oldPassword, newPassword);
                TraceLog.WriteInfo("ChangePassword " + membershipId + " cannot find user in membership");
                return false;
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
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

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
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException("newPassword");
            }


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
