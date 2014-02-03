using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    public class GlobalController : BaseController
    {
        public GlobalController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        { }

        [ChildActionOnly]
        [DonutOutputCache(VaryByParam = "id", Duration = TimeConsts.Week)]
        public ActionResult Loading(string id)
        {
            return PartialView("_Loading", id);
        }

        [ChildActionOnly]
        public ActionResult Tags()
        {
            return PartialView("_Tags");
        }

        /// <summary>
        /// Represent box/item user data
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ShareButtons(string BoxUid, string ItemUid)
        {
            ViewBag.BoxUid = BoxUid;
            ViewBag.ItemUid = ItemUid;
            return PartialView("_Sharebtns");
        }

        [DonutOutputCache(Duration = TimeConsts.Day * 10, VaryByParam = "none")]
        public ActionResult RegisterPopUp()
        {
            return PartialView("_RegisterPopUp");
        }



    }
}
