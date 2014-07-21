using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [AllowAnonymous]
    public class StoreController : BaseController
    {


        [HttpGet, NonAjax, StoreCategories]
        [Route("store/category/{categoryid:int}/{categoryname}", Name = "storeCategory")]
        [Route("store/product/{productid:int}/{productname}")]
        [Route("store")]

        public ActionResult Index()
        {
            return View("Empty");
        }



        [HttpGet, Ajax, ActionName("Index")]
        [Route("store")]
        public ActionResult IndexAjax()
        {
            return PartialView();
        }

        [HttpGet, Ajax]
        public async Task<ActionResult> Products(int? category)
        {
            var products = await ZboxReadService.GetProducts(new GetStoreProductByCategoryQuery(category));
            return this.CdJson(new JsonResponse(true, products));
        }

        //store/product?id=xxx
        [HttpGet, Ajax]
        public ActionResult Product(int id)
        {
            return PartialView();
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