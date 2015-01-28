using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.SiteExtension;
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
        private readonly IItemReadSearchProvider m_ItemSearchService;

        public SearchController(IBoxReadSearchProvider boxSearchService, IItemReadSearchProvider itemSearchService)
        {
            m_BoxSearchService = boxSearchService;
            m_ItemSearchService = itemSearchService;
        }

        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }

        public async Task<JsonResult> Boxes(string term, int page)
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            if (userDetail.UniversityId == null) return JsonError("need university");
            var query = new BoxSearchQuery(term, User.GetUserId(), userDetail.UniversityId.Value, page, 20);
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

            var query = new ItemSearchQuery(term, User.GetUserId(), userDetail.UniversityId.Value, page, 20);
            var retVal = await m_ItemSearchService.SearchItem(query) ?? new List<SearchItems>();
            return JsonOk(retVal.Select(s => new
            {
                s.Image,
                s.Name,
                s.Url
            }));
        }
    }
}