using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Culture;
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

                if (m_ContextBase.User != null && m_ContextBase.User.Identity.IsAuthenticated)
                {
                    var userData = await m_ZboxReadService.GetUserDataAsync(new GetUserDetailsQuery(m_ContextBase.User.GetUserId()));
                    m_LanguageCookie.InjectCookie(userData.Culture);
                    ChangeThreadLanguage(userData.Culture);
                    await Next.Invoke(context);
                    return;
                }

                var language = m_ContextBase.Request.UserLanguages.FirstOrDefault(Languages.CheckIfLanguageIsSupported);
                if (string.IsNullOrEmpty(language))
                {
                    language = Languages.GetDefaultSystemCulture().Culture.First();
                }
                //var country = await GetCountryByIpAsync(m_ContextBase);
                //if (country.ToLower() == "nl")
                //{
                //    country = "gb";
                //}
                //var culture = Languages.GetCultureBaseOnCountry(country);
                m_LanguageCookie.InjectCookie(language);
                ChangeThreadLanguage(language);
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

        //public Task<string> GetCountryByIpAsync(HttpContextBase context)
        //{

        //    var ipNumber = Ip2Long(GetIpFromClient(context));
        //    return m_ZboxReadService.GetLocationByIpAsync(new GetCountryByIpQuery(ipNumber));
        //}

        //public static string GetIpFromClient(HttpContextBase context)
        //{
        //    string userIp = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (string.IsNullOrWhiteSpace(userIp))
        //    {
        //        userIp = context.Request.ServerVariables["REMOTE_ADDR"];
        //    }
        //    if (context.Request.IsLocal)
        //    {
        //        //userIp = "109.158.31.75";
        //        userIp = "81.218.135.73";
        //    }
        //    return userIp;
        //}

        //public static long Ip2Long(string ip)
        //{
        //    double num = 0;
        //    if (!string.IsNullOrEmpty(ip))
        //    {
        //        string[] ipBytes = ip.Split('.');
        //        for (int i = ipBytes.Length - 1; i >= 0; i--)
        //        {
        //            int temp;
        //            int.TryParse(ipBytes[i], out temp);
        //            num += ((temp % 256) * Math.Pow(256, (3 - i)));
        //        }
        //    }
        //    return (long)num;
        //}



    }
}