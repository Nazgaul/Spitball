using System.Globalization;
using System.Threading;
using System.Web;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public static class UserLanguage
    {
        public static void ChangeLanguage(HttpContextBase context, HttpServerUtilityBase server)
        {
            
            if (context.User != null && context.User.Identity.IsAuthenticated)
            {
                var formsAuthenticationService = Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<IFormsAuthenticationService>();
                var userData = formsAuthenticationService.GetUserData();
                if (userData != null)
                {
                    ChangeThreadLanguage(userData.Language);
                    return;
                }
            }

            if (context.Request.QueryString["lang"] != null)
            {
                ChangeThreadLanguage(context.Request.QueryString["lang"]);
                return;
            }
            if (context.Request.Cookies["lang"] != null)
            {
                var value = server.HtmlEncode(context.Request.Cookies["lang"].Value);
                ChangeThreadLanguage(value);
                return;
            }

            if (context.Request.UserLanguages == null) return;
            foreach (var languageWithRating in context.Request.UserLanguages)
            {
                if (string.IsNullOrEmpty(languageWithRating))
                {
                    continue;
                }
                var userLanguage = languageWithRating.Split(';')[0];
                if (userLanguage.StartsWith("nl"))
                {
                    userLanguage = "en-GB";
                }
                if (!Languages.CheckIfLanguageIsSupported(userLanguage)) continue;
                ChangeThreadLanguage(userLanguage);
                break;
            }
        }
        private static void ChangeThreadLanguage(string language)
        {
            if (!Languages.CheckIfLanguageIsSupported(language))
            {
                return;
            }
            if (Thread.CurrentThread.CurrentUICulture.Name == language) return;
            try
            {
                var cultureInfo = new CultureInfo(language);
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
            }
            catch (CultureNotFoundException)
            {
            }
        }
    }
}