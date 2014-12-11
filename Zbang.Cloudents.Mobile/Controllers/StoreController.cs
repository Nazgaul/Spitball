using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mvc4WebRole.Controllers;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [RedirectToDekstopSite]
    [AllowAnonymous]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class StoreController : BaseController
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ChangeThreadLanguage("he-IL");
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

        //[HttpGet, StoreCategories]
        //public async Task<ActionResult> IndexPartial(int? universityId)
        //{
        //    var model = await ZboxReadService.GetBanners(universityId);
        //    return PartialView("Index", model.ToList());
        //}

        //[HttpGet]
        //public async Task<ActionResult> Products(int? categoryId, int? universityId, int? producerId)
        //{
        //    var products = await ZboxReadService.GetProducts(new GetStoreProductsByCategoryQuery(categoryId, universityId, producerId));
        //    return JsonOk(products);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ValidCodeCoupon(int code)
        //{
        //    var retVal = await ZboxReadService.ValidateCoupon(code);
        //    return JsonOk(new { isValid = retVal });
        //}

        //[HttpGet, StoreCategories]
        //public async Task<ActionResult> ProductPartial(long productId, int? universityId)
        //{
        //    var query = new GetStoreProductQuery(productId);
        //    var tModel = ZboxReadService.GetProduct(query);
        //    var tBanners = ZboxReadService.GetBanners(universityId);

        //    await Task.WhenAll(tModel, tBanners);
        //    var model = tModel.Result;
        //    ViewBag.banner =
        //        tBanners.Result.FirstOrDefault(f => f.Location == Zbox.Infrastructure.Enums.StoreBannerLocation.Product);
        //    model.TotalPrice = model.Price + model.DeliveryPrice;
        //    model.Id = productId;
        //    return PartialView("Product", model);
        //}

        //[HttpGet]
        //public async Task<ActionResult> Search(string term, int? universityId)
        //{
        //    if (string.IsNullOrEmpty(term))
        //    {
        //        return JsonError("Provide a term");
        //    }
        //    var query = new SearchProductQuery(term, universityId);
        //    var products = await ZboxReadService.SearchProducts(query);
        //    return JsonOk(products);

        //}

        //[HttpGet, StoreCategories]
        //public async Task<ActionResult> CheckOutPartial(long productId)
        //{
        //    var query = new GetStoreProductQuery(productId);
        //    var model = await ZboxReadService.GetProductCheckOut(query);
        //    if (model == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var serializer = new JsonNetSerializer();

        //    ViewBag.data = serializer.Serialize(model);
        //    ViewBag.NumberOfPayments = model.NumberOfPayments;
        //    return PartialView("CheckOut", model);
        //}

        //[HttpPost]
        //public async Task<ActionResult> Order(StoreOrder model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return JsonError(GetModelStateErrors());
        //    }
        //    await m_QueueProvider.Value.InsertMessageToStoreAsync(new Zbox.Infrastructure.Transport.StoreOrderData(
        //        model.ProductId,
        //        model.IdentityNumber,
        //        model.FirstName,
        //        model.LastName,
        //        model.Address,
        //        model.CreditCardOwnerId,
        //        model.Comments,
        //        model.City,
        //        model.CreditCardOwnerName,
        //        model.CreditCardNumber,
        //        new DateTime(model.ExpirationYear, model.ExpirationMonth, 1),
        //        model.SecurityCode,
        //        model.UniversityId.HasValue ? model.UniversityId.Value : 15,
        //        model.Email,
        //        model.Phone1, model.Phone2,
        //        model.Features,
        //        model.NumberOfPayments));
        //    return JsonOk(new { url = Url.RouteUrl("StoreThanksYou") });
        //}

        //[HttpGet, StoreCategories]
        //public async Task<PartialViewResult> AboutPartial(int? universityId)
        //{
        //    var banner = await ZboxReadService.GetBanners(universityId);
        //    return PartialView("About", banner.FirstOrDefault(f => f.Location == Zbox.Infrastructure.Enums.StoreBannerLocation.Product));
        //}
        //[HttpGet, StoreCategories]

        //public async Task<PartialViewResult> ContactPartial(int? universityId)
        //{
        //    var banner = await ZboxReadService.GetBanners(universityId);
        //    ViewBag.banner = banner.FirstOrDefault(f => f.Location == Zbox.Infrastructure.Enums.StoreBannerLocation.Product);
        //    return PartialView("Contact", new StoreContact());
        //}

        //[HttpPost]
        //[Route("store/contact")]
        //public ActionResult Contact(StoreContact model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return JsonError(GetModelStateErrors());
        //    }
        //    m_QueueProvider.Value.InsertMessageToStoreAsync(
        //        new Zbox.Infrastructure.Transport.StoreContactData(model.Name, model.Phone, model.University,
        //            model.Email, model.Text));
        //    return JsonOk();
        //}

        //[HttpGet, StoreCategories]
        //public ActionResult SalesPartial()
        //{
        //    return PartialView("Sales");
        //}

        //[HttpGet, StoreCategories]
        //public async Task<PartialViewResult> TermsPartial(int? universityId)
        //{
        //    var banner = await ZboxReadService.GetBanners(universityId);
        //    ViewBag.banner = banner.FirstOrDefault(f => f.Location == Zbox.Infrastructure.Enums.StoreBannerLocation.Product);
        //    return PartialView("Terms");
        //}





        //[HttpGet, StoreCategories]
        //public ActionResult ThankyouPartial()
        //{
        //    return PartialView("Thankyou");
        //}
    }
}