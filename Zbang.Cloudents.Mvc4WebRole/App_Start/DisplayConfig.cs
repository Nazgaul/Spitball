using System;
using System.Web;
using System.Web.WebPages;

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

        internal static bool CheckIfIpadView(HttpContextBase context)
        {
            var value =  context.GetOverriddenUserAgent() != null &&
                   context.GetOverriddenUserAgent().IndexOf
                       ("iPad", StringComparison.OrdinalIgnoreCase) >= 0;
            return value;
        }

        internal static bool CheckIfMobileView(HttpContextBase c)
        {
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