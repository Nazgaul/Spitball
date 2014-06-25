using System.Linq;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

namespace Zbang.Cloudents.Mvc4WebRole.App_Start
{
    public class ViewConfig
    {
        public static void RegisterEngineAndViews()
        {
            ViewEngines.Engines.Clear();

            var razorViewEngine = new RazorViewEngine
            {
                ViewLocationCache = new CloudentsViewLocationCache()
            };

          
            razorViewEngine.AreaMasterLocationFormats = new string[0];
            razorViewEngine.AreaPartialViewLocationFormats = new string[0];
            razorViewEngine.AreaViewLocationFormats = new string[0];

            var customPartialLocation = new[] {
                   "~/Views/Shared/Partials/{0}.cshtml"
            };
            
            
            razorViewEngine.PartialViewLocationFormats = 
                razorViewEngine.PartialViewLocationFormats.Union(customPartialLocation).ToArray();
            ViewEngines.Engines.Add(razorViewEngine);
        }
    }

    

}