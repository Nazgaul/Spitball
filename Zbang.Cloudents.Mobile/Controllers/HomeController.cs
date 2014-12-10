using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mobile.Js.Resources;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Cloudents.Mobile.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }


        [DonutOutputCache(Duration = TimeConsts.Day,
           VaryByParam = "none", Location = OutputCacheLocation.Server,
           VaryByCustom = CustomCacheKeys.Lang, Order = 2)]
        [CacheFilter(Duration = TimeConsts.Day)]
        public ActionResult JsResources()
        {
            //var rm = new ResourceManager("Zbang.Cloudents.Mvc4WebRole.Js.Resources.JsResources", Assembly.GetExecutingAssembly());
            var x = typeof(JsResources);
            var sb = new StringBuilder();
            sb.Append("JsResources={");
            foreach (var p in x.GetProperties())
            {

                var s = p.GetValue(null, null);
                if (s is string)
                {
                    sb.Append("\"" + p.Name + "\":\"" +
                              s.ToString().Replace("\r\n", @"\n").Replace("\n", @"\n").Replace("\"", @"\""") +
                              "\",");
                    sb.AppendLine();
                }


            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return Content(sb.ToString(), "application/javascript");
        }

        [ChildActionOnly]
        public ActionResult AntiForgeryToken()
        {
            return PartialView();
        }
    }
}