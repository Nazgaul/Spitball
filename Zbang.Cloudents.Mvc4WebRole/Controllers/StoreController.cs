using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [AllowAnonymous]
    public class StoreController : BaseController
    {


        // GET: Shopping
        [HttpGet, NonAjax]
        public ActionResult Index()
        {
            return View("Empty");
        }

        [HttpGet, Ajax, ActionName("Index")]
        public ActionResult IndexAjax()
        {
            return PartialView();
        }

        [HttpGet, Ajax]
        public async Task<ActionResult> Products(int? category)
        {
            var products = await ZboxReadService.GetProducts();
            return this.CdJson(new JsonResponse(true, products));
        }

        public ActionResult Product()
        {
            return View();
        }

        public ActionResult CheckOut()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Sales()
        {
            return View();
        }
    }
}