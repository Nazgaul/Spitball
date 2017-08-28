using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class LanguageMiddleware : OwinMiddleware
    {
        private readonly IZboxReadService m_ZboxReadService;
        private readonly HttpContextBase m_ContextBase;
        private readonly ILanguageCookieHelper m_LanguageCookie;

        public LanguageMiddleware(OwinMiddleware next, IZboxReadService zboxReadService,
            HttpContextBase contextBase,
            ILanguageCookieHelper languageCookie)
            : base(next)
        {
            m_ZboxReadService = zboxReadService;
            m_ContextBase = contextBase;
            m_LanguageCookie = languageCookie;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (!string.IsNullOrEmpty(context.Request.Query["lang"]))
            {
                var culture = ChangeThreadLanguage(context.Request.Query["lang"]);
                m_LanguageCookie.InjectCookie(culture);
                await Next.Invoke(context);
                return;
            }
            var lang = m_LanguageCookie.ReadCookie();
            if (lang == null)
            {
                if (m_ContextBase.User?.Identity.IsAuthenticated == true)
                {
                    var userData = await m_ZboxReadService.GetUserDataAsync(new QueryBaseUserId(m_ContextBase.User.GetUserId()));
                    m_LanguageCookie.InjectCookie(userData.Culture);
                    ChangeThreadLanguage(userData.Culture);
                    await Next.Invoke(context);
                    return;
                }

                if (m_ContextBase.Request.UserLanguages != null)
                {
                    var language = m_ContextBase.Request.UserLanguages.FirstOrDefault(Languages.CheckIfLanguageIsSupported);
                    if (string.IsNullOrEmpty(language))
                    {
                        language = Languages.DefaultSystemCulture.Culture.First();
                    }
                    m_LanguageCookie.InjectCookie(language);
                    ChangeThreadLanguage(language);
                }
                await Next.Invoke(context);
                return;
            }
            ChangeThreadLanguage(lang);
            await Next.Invoke(context);
        }

        public static string ChangeThreadLanguage(string language)
        {
            if (!Languages.CheckIfLanguageIsSupported(language))
            {
                return Thread.CurrentThread.CurrentUICulture.Name;
            }
            if (Thread.CurrentThread.CurrentUICulture.Name == language)
            {
                return Thread.CurrentThread.CurrentUICulture.Name;
            }
            try
            {
                var cultureInfo = new CultureInfo(language);
                ChangeThreadCulture(cultureInfo);
            }
            catch (CultureNotFoundException)
            {

            }
            return Thread.CurrentThread.CurrentUICulture.Name;
        }

        private static void ChangeThreadCulture(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
        }
    }
}