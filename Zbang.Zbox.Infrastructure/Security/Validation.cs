using Zbang.Zbox.Infrastructure.Security.Resources;

namespace Zbang.Zbox.Infrastructure.Security
{
    #region Validation

    public static class AccountValidation
    {
        public enum AccountError
        {
            InvalidPassword,
            InvalidEmail
        }

        public static string ErrorCodeToString(AccountError createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                //case MembershipCreateStatus.DuplicateUserName:
                //    return "This user name is already taken."; //no need

                //case MembershipCreateStatus.DuplicateEmail:
                //    return Validation.DuplicateEmail;

                case AccountError.InvalidPassword:
                    return Validation.InvalidPassword;

                case AccountError.InvalidEmail:
                    return Validation.InvalidEmail;

                //case MembershipCreateStatus.InvalidAnswer: //no need
                //    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                //case MembershipCreateStatus.InvalidQuestion://no need
                //    return "The password retrieval question provided is invalid. Please check the value and try again.";

                //case MembershipCreateStatus.InvalidUserName:
                //    return Validation.InvalidUserName;

                //case MembershipCreateStatus.ProviderError:
                //    return Validation.ProviderError;

                //case MembershipCreateStatus.UserRejected:
                //    return Validation.UserRejected;
                default:
                    return Validation.DefaultError;
            }
        }

        public static string Localize(string error, string userName)
        {
            if ($"Name {userName} is already taken." == error)
            {
                return Validation.DuplicateEmail;
            }
            return error;
        }
    }

    #endregion
}