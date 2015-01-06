using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Extensions;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [ZboxAuthorize]
    [NoUniversity]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class SearchController : BaseController
    {
        private readonly IBoxReadSearchProvider m_BoxSearchService;

        public SearchController(IBoxReadSearchProvider boxSearchService)
        {
            m_BoxSearchService = boxSearchService;
        }

        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }

        public async Task<JsonResult> Boxes(string term, int page)
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            if (userDetail.UniversityId == null) return JsonError("need university");
            var query = new BoxSearchQuery(term, userDetail.UniversityId.Value, User.GetUserId(), page, 20);
            var retVal = await m_BoxSearchService.SearchBox(query) ?? new List<SearchBoxes>();
            return JsonOk(retVal.Select(s => new
            {
                s.Url,
                s.Image,
                s.Name
            }));
        }
        public async Task<JsonResult> Items(string term, int page)
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            if (userDetail.UniversityId == null) return JsonError("need university");
            var query = new GroupSearchQuery(term, userDetail.UniversityId.Value, User.GetUserId(), page, 20);
            var retVal = await ZboxReadService.SearchItems(query);
            return JsonOk(retVal.Select(s => new
            {
                s.Image,
                s.Name,
                s.Url
            }));
        }
    }
}