﻿using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Routing;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public static class UserLanguage
    {
        public const string CookieName = "l1";
        public static void ChangeLanguage(HttpContextBase context, HttpServerUtilityBase server, RouteData route)
        {
            var cookie = new CookieHelper(context);
            if (route != null && route.Values["lang"] != null)
            {
                var culture = ChangeThreadLanguage(route.Values["lang"].ToString());
                cookie.InjectCookie(CookieName, culture);
                return;
            }
            if (context.Request.QueryString["lang"] != null)
            {
                var culture = ChangeThreadLanguage(context.Request.QueryString["lang"]);
                cookie.InjectCookie(CookieName, culture);
                return;
            }
            var lang = cookie.ReadCookie<string>(CookieName);
            if (lang == null)
            {
                if (context.User != null && context.User.Identity.IsAuthenticated)
                {
                    var zboxReadService = Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<IZboxReadService>();
                    var userData = zboxReadService.GetUserData(new Zbox.ViewModel.Queries.GetUserDetailsQuery(context.User.GetUserId()));
                    cookie.InjectCookie(CookieName, userData.Culture);
                    ChangeThreadLanguage(userData.Culture);
                    return;
                }
                var country = GetCountryByIp(context);
                if (country.ToLower() == "nl")
                {
                    country = "gb";
                }
                var culture = Languages.GetCultureBaseOnCountry(country);
                cookie.InjectCookie(CookieName, culture.Name);
                ChangeThreadCulture(culture);
                return;
            }
            ChangeThreadLanguage(lang);
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

        public static string GetCountryByIp(HttpContextBase context)
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
            var zboxReadService = Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<IZboxReadService>();
            return zboxReadService.GetLocationByIp(ipNumber);
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