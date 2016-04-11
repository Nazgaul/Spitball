using System.Linq;
using System.Web.Mvc;
using StackExchange.Profiling.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class ViewConfig
    {
        public static void RegisterEngineAndViews()
        {
            ViewEngines.Engines.Clear();

            var razorViewEngine = new RazorViewEngine
            {
                ViewLocationCache = new CloudentsViewLocationCache(),
                AreaMasterLocationFormats = new string[0],
                AreaPartialViewLocationFormats = new string[0],
                AreaViewLocationFormats = new string[0],
                FileExtensions = new[]
                {
                    "cshtml"
                }
            };


            var customPartialLocation = new[] {
                   "~/Views/Shared/Partials2/{0}.cshtml"
            };
            
            
            
            razorViewEngine.PartialViewLocationFormats = 
                razorViewEngine.PartialViewLocationFormats.Union(customPartialLocation).ToArray();
#if DEBUG
            ViewEngines.Engines.Add(new ProfilingViewEngine(razorViewEngine));
#else
            ViewEngines.Engines.Add(razorViewEngine);
#endif
        }
    }

    

}