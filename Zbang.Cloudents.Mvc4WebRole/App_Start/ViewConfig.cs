using System.Linq;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public class ViewConfig
    {
        public static void RegisterEngineAndViews()
        {
            ViewEngines.Engines.Clear();

            var razorViewEngine = new RazorViewEngine
            {
                ViewLocationCache = new CloudentsViewLocationCache(),
                AreaMasterLocationFormats = new string[0],
                AreaPartialViewLocationFormats = new string[0],
                AreaViewLocationFormats = new string[0]
            };


            var customPartialLocation = new[] {
                   "~/Views/Shared/Partials/{0}.cshtml"
            };
            
            
            razorViewEngine.PartialViewLocationFormats = 
                razorViewEngine.PartialViewLocationFormats.Union(customPartialLocation).ToArray();
            ViewEngines.Engines.Add(razorViewEngine);
        }
    }

    

}