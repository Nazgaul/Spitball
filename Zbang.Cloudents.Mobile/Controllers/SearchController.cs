using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Extensions;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [ZboxAuthorize]
    [NoUniversity]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class SearchController : BaseController
    {
        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }

        public async Task<JsonResult> Boxes(string term, int page)
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            if (userDetail.UniversityId == null) return JsonError("need university");
            var query = new GroupSearchQuery(term, userDetail.UniversityId.Value, User.GetUserId(), page, 20);
            var retVal = await ZboxReadService.SearchBoxes(query);
            return JsonOk(retVal);
        }
        public async Task<JsonResult> Items(string term, int page)
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            if (userDetail.UniversityId == null) return JsonError("need university");
            var query = new GroupSearchQuery(term, userDetail.UniversityId.Value, User.GetUserId(), page, 20);
            var retVal = await ZboxReadService.SearchItems(query);
            return JsonOk(retVal);
        }
    }
}