using System.Web.Security;
using Zbang.Zbox.Infrastructure.Security.Resources;

namespace Zbang.Zbox.Infrastructure.Security
{

    #region Validation

    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "This user name is already taken."; //no need

                case MembershipCreateStatus.DuplicateEmail:
                    return Validation.DuplicateEmail;

                case MembershipCreateStatus.InvalidPassword:
                    return Validation.InvalidPassword;

                case MembershipCreateStatus.InvalidEmail:
                    return Validation.InvalidEmail;

                case MembershipCreateStatus.InvalidAnswer: //no need
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion://no need
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return Validation.InvalidUserName;

                case MembershipCreateStatus.ProviderError:
                    return Validation.ProviderError;

                case MembershipCreateStatus.UserRejected:
                    return Validation.UserRejected;
                default:
                    return Validation.DefaultError;
            }
        }
    }


    #endregion
}