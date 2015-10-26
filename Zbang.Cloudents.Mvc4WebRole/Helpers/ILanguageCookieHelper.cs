using System.Globalization;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public interface ILanguageCookieHelper
    {
        void InjectCookie(CultureInfo value);
        void InjectCookie(string value);
        string ReadCookie();
    }
}