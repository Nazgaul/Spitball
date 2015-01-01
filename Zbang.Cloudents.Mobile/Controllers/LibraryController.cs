using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mvc4WebRole.Controllers;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    public class LibraryController : BaseController
    {
        private readonly Lazy<IUniversityReadSearchProvider> m_UniversitySearch;

        public LibraryController(Lazy<IUniversityReadSearchProvider> universitySearch)
        {
            m_UniversitySearch = universitySearch;
        }

        //[UserNavNWelcome]
        [HttpGet]
        [RedirectToDesktopSite]
        public ActionResult Index()
        {
            return new EmptyResult();

        }

        [HttpGet]
        public async Task<JsonResult> SearchUniversity(string term, int page)
        {
            if (string.IsNullOrEmpty(term))
            {
                return JsonError();
            }
            try
            {
                var retVal = await m_UniversitySearch.Value.SearchUniversity(new UniversitySearchQuery(term, 20, page));
                return JsonOk(retVal);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("SeachUniversity term:  " + term, ex);
                return JsonError();
            }
        }

    }
}
