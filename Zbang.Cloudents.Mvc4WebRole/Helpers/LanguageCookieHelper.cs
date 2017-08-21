using System.Globalization;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class LanguageCookieHelper : ILanguageCookieHelper
    {
        private readonly ICookieHelper m_CookieHelper;
        const string CookieName = "l3";
        public LanguageCookieHelper(ICookieHelper cookieHelper)
        {
            m_CookieHelper = cookieHelper;
        }

        public void InjectCookie(CultureInfo value)
        {
            m_CookieHelper.InjectCookie(CookieName, value.Name, false);
        }

        public void InjectCookie(string value)
        {
            m_CookieHelper.InjectCookie(CookieName, value, false);
        }

        public string ReadCookie()
        {
            return m_CookieHelper.ReadCookie<string>(CookieName);
        }
    }
}