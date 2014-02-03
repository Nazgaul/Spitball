using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace Zbang.Cloudents.Mvc4WebRole.App_Start
{
    public static class DisplayConfig
    {
        internal static void RegisterDisplays()
        {
            //Iphone app is not going to mobile site
            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("mobile")
            {
                ContextCondition = (
                c => c.GetOverriddenUserAgent() != null &&
                    c.GetOverriddenUserAgent().IndexOf("iPhone", StringComparison.OrdinalIgnoreCase) >= 0)
            });
            DisplayModeProvider.Instance.Modes.Insert(2, new DefaultDisplayMode("mobile")
            {
                ContextCondition = (c => c.GetOverriddenUserAgent() != null &&
                    c.GetOverriddenUserAgent().IndexOf("Mobile", StringComparison.OrdinalIgnoreCase) >= 0)
            });
            // DisplayModeProvider.Instance.Modes.Clear();
            //DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode(string.Empty)
            //{
            //    ContextCondition = (c => true)
            //});

            //Ipad should go to regular site
            DisplayModeProvider.Instance.Modes.Insert(1, new DefaultDisplayMode(string.Empty)
            {
                ContextCondition = (context =>
                    context.GetOverriddenUserAgent() != null &&
                    context.GetOverriddenUserAgent().IndexOf
                    ("iPad", StringComparison.OrdinalIgnoreCase) >= 0)

            });

            //DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("mobile")
            //{
            //    ContextCondition = (context => isAndroid(context))
            //});
        }

        //internal static bool isAndroid(HttpContextBase context)
        //{
        //    return context.GetOverriddenUserAgent().IndexOf
        //            ("Android", StringComparison.OrdinalIgnoreCase) >= 0;
        //}


    }
}