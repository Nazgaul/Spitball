using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Infrastructure.Search;
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
            var universityId = User.GetUniversityId();
            //var userDetail = FormsAuthenticationService.GetUserData();
            if (!universityId.HasValue) return JsonError("need university");
            var query = new SearchQuery(term, User.GetUserId(), universityId.Value, page, 20);
            var retVal = await m_BoxSearchService.SearchBox(query, Response.ClientDisconnectedToken) ?? new List<SearchBoxes>();
            return JsonOk(retVal.Select(s => new
            {
                s.Url,
                s.Name
            }));
        }
        public async Task<JsonResult> Items(string term, int page)
        {
            var universityId = User.GetUniversityId();
            if (!universityId.HasValue) return JsonError("need university");

            var query = new SearchQuery(term, User.GetUserId(), universityId.Value, page, 20);
            var retVal = await m_ItemSearchService.SearchItem(query, Response.ClientDisconnectedToken) ?? new List<SearchItems>();
            return JsonOk(retVal.Select(s => new
            {
                s.Image,
                s.Name,
                s.Url
            }));
        }
    }
}