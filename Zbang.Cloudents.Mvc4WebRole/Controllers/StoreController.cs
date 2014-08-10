using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [AllowAnonymous]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class StoreController : BaseController
    {
        private readonly Lazy<IQueueProvider> m_QueueProvider;
        public StoreController(Lazy<IQueueProvider> queueProvider)
        {
            m_QueueProvider = queueProvider;


        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ChangeThreadLanguage("he-IL");
        }

        [HttpGet, NonAjax]
        [Route("store/category/{categoryid:int}", Name = "storeCategory")]
        [Route("store/product/{productid:int}/{productname}")]
        [Route("store/terms", Name = "StoreTerms")]
        [Route("store", Name = "StoreRoot")]
        [Route("store/about", Name = "StoreAbout")]
        [Route("store/contact", Name = "StoreContact")]
        [Route("store/sales")]
        [Route("store/thankyou", Name = "StoreThanksYou")]
        [Route("store/checkout/{id:int}", Name = "StoreCheckout")]
        public async Task<ActionResult> Index(int? universityId)
        {
            if (User.Identity.IsAuthenticated && !universityId.HasValue)
            {
                var userDetail = FormsAuthenticationService.GetUserData();
                var universityWrapper = userDetail.UniversityWrapperId ?? userDetail.UniversityId.Value;
                var storeUniversityId = await ZboxReadService.CloudentsUniversityToStoreUniversity(universityWrapper);
                return RedirectToAction("Index", new { universityId = storeUniversityId });
            }
            return View("Empty");
        }

        [HttpGet, Ajax, ActionName("Index"), StoreCategories]
        [Route("store")]
        public async Task<ActionResult> IndexAjax(int? universityId)
        {
            var model = await ZboxReadService.GetBanners(universityId);
            return PartialView(model.ToList());
        }

        [HttpGet, Ajax]
        public async Task<ActionResult> Products(int? categoryId, int? universityId)
        {
            var products = await ZboxReadService.GetProducts(new GetStoreProductsByCategoryQuery(categoryId, universityId));
            return this.CdJson(new JsonResponse(true, products));
        }

        [HttpGet, Ajax]
        public async Task<ActionResult> ValidCodeCoupon(int code)
        {
            var retVal = await ZboxReadService.ValidateCoupon(code);
            return this.CdJson(new JsonResponse(true, new { isValid = retVal }));
        }

        [HttpGet, Ajax, StoreCategories]
        public async Task<ActionResult> Product(long id, int? universityId)
        {
            var query = new GetStoreProductQuery(id);
            var tModel = ZboxReadService.GetProduct(query);
            var tBanners = ZboxReadService.GetBanners(universityId);

            await Task.WhenAll(tModel, tBanners);
            var model = tModel.Result;
            ViewBag.banner =
                tBanners.Result.FirstOrDefault(f => f.Location == Zbox.Infrastructure.Enums.StoreBannerLocation.Product);
            model.TotalPrice = model.Price + model.DeliveryPrice;
            model.Id = id;
            return PartialView(model);
        }

        [HttpGet, Ajax]
        public async Task<ActionResult> Search(string term, int? universityId)
        {
            if (string.IsNullOrEmpty(term))
            {
                return this.CdJson(new JsonResponse(false, "Provide a term"));
            }
            var query = new SearchProductQuery(term, universityId);
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
            ViewBag.NumberOfPayments = model.NumberOfPayments;
            return PartialView(model);
        }

        [Ajax, HttpPost]
        public async Task<ActionResult> Order(StoreOrder model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetModelStateErrors()));
            }
            await m_QueueProvider.Value.InsertMessageToStoreAsync(new Zbox.Infrastructure.Transport.StoreOrderData(
                model.ProductId,
                model.IdentityNumber,
                model.FirstName,
                model.LastName,
                model.Address,
                model.CreditCardOwnerId,
                model.Comments,
                model.City,
                model.CreditCardOwnerName,
                model.CreditCardNumber,
                new DateTime(model.ExpirationYear, model.ExpirationMonth, 1),
                model.SecurityCode,
                model.UniversityId.HasValue ? model.UniversityId.Value : 15,
                model.Email,
                model.Phone1, model.Phone2,
                model.Features,
                model.NumberOfPayments));
            return this.CdJson(new JsonResponse(true, new { url = Url.RouteUrl("StoreThanksYou") }));
        }

        [Ajax, HttpGet, StoreCategories]
        [Route("store/about")]
        public async Task<PartialViewResult> About(int? universityId)
        {
            var banner = await ZboxReadService.GetBanners(universityId);
            return PartialView(banner.FirstOrDefault(f => f.Location == Zbox.Infrastructure.Enums.StoreBannerLocation.Product));
        }
        [Ajax, HttpGet, StoreCategories]
        [Route("store/contact")]

        public async Task<PartialViewResult> Contact(int? universityId)
        {
            var banner = await ZboxReadService.GetBanners(universityId);
            ViewBag.banner = banner.FirstOrDefault(f => f.Location == Zbox.Infrastructure.Enums.StoreBannerLocation.Product);
            return PartialView(new StoreContact());
        }

        [Ajax, HttpPost]
        [Route("store/contact")]
        public ActionResult Contact(StoreContact model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetModelStateErrors()));
            }
            m_QueueProvider.Value.InsertMessageToStoreAsync(
                new Zbox.Infrastructure.Transport.StoreContactData(model.Name, model.Phone, model.University,
                    model.Email, model.Text));
            return this.CdJson(new JsonResponse(true));
        }

        [Ajax, HttpGet, StoreCategories]
        [Route("store/sales")]
        public ActionResult Sales()
        {
            return PartialView();
        }

        [Ajax, HttpGet, StoreCategories]
        [Route("store/terms")]
        public ActionResult Terms()
        {
            return PartialView();
        }





        [Ajax, HttpGet, StoreCategories]
        [Route("store/thankyou")]
        public ActionResult Thankyou()
        {
            return PartialView();
        }
    }
}