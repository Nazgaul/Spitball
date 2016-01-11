using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class ThemeCookieHelper : IThemeCookieHelper
    {
        private readonly ICookieHelper m_CookieHelper;
        const string CookieName = "t";
        public ThemeCookieHelper(ICookieHelper cookieHelper)
        {
            m_CookieHelper = cookieHelper;
        }

        public void InjectCookie(Theme value)
        {
            m_CookieHelper.InjectCookie(CookieName, value.GetStringValueLowerCase(), false);
        }
        public Theme? ReadCookie()
        {
            var cookieValue =  m_CookieHelper.ReadCookie<string>(CookieName);
            Theme retVal;
            if (Enum.TryParse(cookieValue, true, out retVal))
            {
                return retVal;    
            }
            return null;
        }
    }
}