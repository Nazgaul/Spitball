using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [AllowAnonymous]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class StoreController : BaseController
    {


        [HttpGet, NonAjax]
        [Route("store/category/{categoryid:int}/{categoryname}", Name = "storeCategory")]
        [Route("store/product/{productid:int}/{productname}")]
        [Route("store/terms")]
        [Route("store")]
        [Route("store/about")]
        [Route("store/contact")]
        [Route("store/sales")]
        [Route("store/thankyou")]
        [Route("store/checkout/{id:int}", Name = "StoreCheckout")]
        public ActionResult Index()
        {
            return View("Empty");
        }



        [HttpGet, Ajax, ActionName("Index"), StoreCategories]
        [Route("store")]
        public async Task<ActionResult> IndexAjax()
        {
            var model = await ZboxReadService.GetBanners();
            return PartialView(model.ToList());
        }

        [HttpGet, Ajax]
        public async Task<ActionResult> Products(int? categoryId)
        {
            var products = await ZboxReadService.GetProducts(new GetStoreProductsByCategoryQuery(categoryId));
            return this.CdJson(new JsonResponse(true, products));
        }

        [HttpGet, Ajax, StoreCategories]
        public async Task<ActionResult> Product(long id)
        {
            var query = new GetStoreProductQuery(id);
            var tModel =  ZboxReadService.GetProduct(query);
            var tBanners =  ZboxReadService.GetBanners();

            await Task.WhenAll(tModel, tBanners);
            var model = tModel.Result;
            ViewBag.banner =
                tBanners.Result.FirstOrDefault(f => f.Location == Zbox.Infrastructure.Enums.StoreBannerLocation.Product);
            model.TotalPrice = model.Price + model.DeliveryPrice;
            model.Id = id;
            return PartialView(model);
        }

        [HttpGet, Ajax]
        public async Task<ActionResult> Search(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return this.CdJson(new JsonResponse(false, "Provide a term"));
            }
            var query = new SearchProductQuery(term);
            var products = await ZboxReadService.SearchProducts(query);
            return this.CdJson(new JsonResponse(true, products));

        }

        [Ajax, HttpGet, StoreCategories]
        public async Task<ActionResult> CheckOut(long id)
        {
            var query = new GetStoreProductQuery(id);
            var model = await ZboxReadService.GetProductCheckOut(query);
            var serializer = new JsonNetSerializer();
            ViewBag.data = serializer.Serialize(model);
            return PartialView(model);
        }

        [Ajax, HttpGet, StoreCategories]

        public ActionResult About()
        {
            return PartialView();
        }
        [Ajax, HttpGet, StoreCategories]

        public ActionResult Contact()
        {
            return PartialView();
        }
        [Ajax, HttpGet, StoreCategories]
        public ActionResult Sales()
        {
            return PartialView();
        }

        [Ajax, HttpGet, StoreCategories]
        public ActionResult Terms()
        {
            return PartialView();
        }

        [Ajax, HttpGet, StoreCategories]
        public ActionResult Thankyou()
        {
            return PartialView();
        }
    }
}