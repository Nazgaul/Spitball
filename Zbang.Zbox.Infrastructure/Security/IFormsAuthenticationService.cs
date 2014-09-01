
namespace Zbang.Zbox.Infrastructure.Security
{
    public interface IFormsAuthenticationService
    {
        //void SignIn(string userName, bool createPersistentCookie);
        void SignIn(long userId, bool rememberMe, UserDetail userDetail);
        void SignOut();

        UserDetail GetUserData();

        //void VerifyEmail();
        //void ChangeNameAndEmail(long? universityId);
        void ChangeLanguage(string newLanguage);

        //void ChangeUniversity(long universityId);
        void ChangeUniversity(long? universityId);
    }
}
