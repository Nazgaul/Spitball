using System;
using System.Web;
using System.Web.WebPages;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class DisplayConfig
    {
        internal static void RegisterDisplays()
        {
            //Iphone app is not going to mobile site
            //DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("mobile")
            //{
            //    ContextCondition = (
            //    c => c.GetOverriddenUserAgent() != null &&
            //        c.GetOverriddenUserAgent().IndexOf("iPhone", StringComparison.OrdinalIgnoreCase) >= 0)
            //});
            //DisplayModeProvider.Instance.Modes.Insert(2, new DefaultDisplayMode("mobile")
            //{
            //    ContextCondition = (c => c.GetOverriddenUserAgent() != null &&
            //        c.GetOverriddenUserAgent().IndexOf("Mobile", StringComparison.OrdinalIgnoreCase) >= 0)
            //});
            if (IsMobileDisabled())
            {
                DisplayModeProvider.Instance.Modes.Clear();
                DisplayModeProvider.Instance.Modes.Add(new DefaultDisplayMode(string.Empty));
                return;
            }

            DisplayModeProvider.Instance.Modes.Insert(1, new DefaultDisplayMode("mobile")
            {
                ContextCondition = (c => CheckIfMobileView(c))
            });
            // DisplayModeProvider.Instance.Modes.Clear();
            //DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode(string.Empty)
            //{
            //    ContextCondition = (c => true)
            //});

            //Ipad should go to regular site
            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode(string.Empty)
            {
                ContextCondition = (context => CheckIfIpadView(context))
            });



            //DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("mobile")
            //{
            //    ContextCondition = (context => isAndroid(context))
            //});
        }

        private static bool IsMobileDisabled()
        {
            var mobileDisabled = ConfigFetcher.Fetch("IsMobileDisabled");
            bool retVal;
            bool.TryParse(mobileDisabled, out retVal);
            return retVal;
        }

        internal static bool CheckIfIpadView(HttpContextBase context)
        {
            if (IsMobileDisabled())
            {
                return false;
            }
            var value = context.GetOverriddenUserAgent() != null &&
                   context.GetOverriddenUserAgent().IndexOf
                       ("iPad", StringComparison.OrdinalIgnoreCase) >= 0;
            return value;
        }

        internal static bool CheckIfMobileView(HttpContextBase c)
        {
            if (IsMobileDisabled())
            {
                return false;
            }
            if (CheckIfIpadView(c))
            {
                return false;
            }

            return (
                   c.GetOverriddenUserAgent() != null &&
                   c.GetOverriddenUserAgent().IndexOf("iPhone", StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (c.GetOverriddenUserAgent() != null &&
                    c.GetOverriddenUserAgent().IndexOf("Mobile", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //internal static bool isAndroid(HttpContextBase context)
        //{
        //    return context.GetOverriddenUserAgent().IndexOf
        //            ("Android", StringComparison.OrdinalIgnoreCase) >= 0;
        //}


    }
}