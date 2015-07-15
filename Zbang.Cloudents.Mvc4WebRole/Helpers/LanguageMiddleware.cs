using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Microsoft.Owin;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Culture;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class LanguageMiddleware : OwinMiddleware
    {
        private readonly IZboxReadService m_ZboxReadService;
        private readonly HttpContextBase m_ContextBase;
        private readonly ILanguageCookieHelper m_LanguageCookie;

        public LanguageMiddleware(OwinMiddleware next, IZboxReadService zboxReadService, HttpContextBase contextBase, ILanguageCookieHelper languageCookie)
            : base(next)
        {
            m_ZboxReadService = zboxReadService;
            m_ContextBase = contextBase;
            m_LanguageCookie = languageCookie;
        }

        public override async System.Threading.Tasks.Task Invoke(IOwinContext context)
        {
            //if (route != null && route.Values["lang"] != null)
            //{
            //    var culture = ChangeThreadLanguage(route.Values["lang"].ToString());
            //    InsertCookie(culture, context);
            //    //cookie.InjectCookie(CookieName, culture, false);
            //    return;
            //}
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

                if (m_ContextBase.User != null && m_ContextBase.User.Identity.IsAuthenticated)
                {
                    var userData = await m_ZboxReadService.GetUserDataAsync(new Zbox.ViewModel.Queries.GetUserDetailsQuery(m_ContextBase.User.GetUserId()));
                    m_LanguageCookie.InjectCookie(userData.Culture);
                    ChangeThreadLanguage(userData.Culture);
                    await Next.Invoke(context);
                    return;
                }
                var country = GetCountryByIp(m_ContextBase);
                if (country.ToLower() == "nl")
                {
                    country = "gb";
                }
                var culture = Languages.GetCultureBaseOnCountry(country);
                m_LanguageCookie.InjectCookie(culture);
                ChangeThreadCulture(culture);
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

        public string GetCountryByIp(HttpContextBase context)
        {
            string userIp = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(userIp))
            {
                userIp = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (context.Request.IsLocal)
            {
                //userIp = "109.158.31.75";
                userIp = "81.218.135.73";
            }
            var ipNumber = Ip2Long(userIp);
            return m_ZboxReadService.GetLocationByIp(ipNumber);
        }

        private static long Ip2Long(string ip)
        {
            double num = 0;
            if (!string.IsNullOrEmpty(ip))
            {
                string[] ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }
            }
            return (long)num;
        }


       
    }
}