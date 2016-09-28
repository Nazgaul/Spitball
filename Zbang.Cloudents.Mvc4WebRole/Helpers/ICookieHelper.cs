namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public interface ICookieHelper
    {
        void InjectCookie<T>(string cookieName, T cookieData, bool httpOnly = true) where T : class;
        T ReadCookie<T>(string cookieName) where T : class;
        void RemoveCookie(string cookieName);
    }
}