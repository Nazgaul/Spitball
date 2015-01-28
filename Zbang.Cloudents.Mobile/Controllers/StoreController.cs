using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.SiteExtension;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [RedirectToDesktopSite]
    [AllowAnonymous]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class StoreController : BaseController
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            UserLanguage.ChangeThreadLanguage("he-IL");
        }

        [HttpGet, NoUniversity]
        [Route("store/category/{categoryid:int}", Name = "storeCategory")]
        [Route("store/product/{productid:int}/{productname}")]
        [Route("store/terms", Name = "StoreTerms")]
        [Route("store", Name = "StoreRoot")]
        [Route("store/about", Name = "StoreAbout")]
        [Route("store/contact", Name = "StoreContact")]
        [Route("store/sales")]
        [Route("store/thankyou", Name = "StoreThanksYou")]
        [Route("store/checkout/{id:int}", Name = "StoreCheckout")]
        public ActionResult Index(int? universityId, int? producerId)
        {
            return new EmptyResult();
        }
    }
}