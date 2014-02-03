using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Zbox.Mvc3WebRole.App_Start
{
    public class ViewConfig
    {
        public static void RegisterEngineAndViews()
        {
            ViewEngines.Engines.Clear();

            //TODO: Maybe should remove caching to out caching.
            var razorViewEngine = new RazorViewEngine
            {
                ViewLocationCache = new DefaultViewLocationCache(TimeSpan.FromDays(1))
            };

            var customPartialLocation = new[] {
                   "~/Views/Shared/Partials/{0}.cshtml" 
            };

            razorViewEngine.PartialViewLocationFormats = razorViewEngine.PartialViewLocationFormats.Union(customPartialLocation).ToArray();
            ViewEngines.Engines.Add(razorViewEngine);
        }
    }
}