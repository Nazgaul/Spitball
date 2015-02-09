
namespace Zbang.Zbox.Infrastructure.Security
{
    public interface IFormsAuthenticationService
    {
        void SignIn(long userId, bool rememberMe, UserDetail userDetail);
        void SignOut();

        UserDetail GetUserData();

        void ChangeUniversity(long? universityId, long? universityDataId);
    }
}
