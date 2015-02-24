using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class LibraryController : BaseController
    {
        private readonly Lazy<IUniversityReadSearchProvider> m_UniversitySearch;
        private readonly Lazy<IFacebookService> m_FacebookService;

        public LibraryController(
            Lazy<IUniversityReadSearchProvider> universitySearch,
            Lazy<IFacebookService> facebookService)
        {
            m_UniversitySearch = universitySearch;
            m_FacebookService = facebookService;
        }

        //[UserNavNWelcome]
        [HttpGet]
        [RedirectToDesktopSite]
        public ActionResult Index()
        {
            return new EmptyResult();

        }

        [HttpGet]
        public PartialViewResult ChoosePartial()
        {
            return PartialView();
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



        [HttpGet]
        public async Task<JsonResult> GetFriends(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                return Json(null);
            }
            try
            {
                var friendsId = await m_FacebookService.Value.GetFacebookUserFriends(authToken);
                var facebookFriendDatas = friendsId as FacebookFriendData[] ?? friendsId.ToArray();
                var suggestedUniversity =
                    await ZboxReadService.GetUniversityListByFriendsIds(facebookFriendDatas.Select(s => s.Id));

                foreach (var university in suggestedUniversity)
                {
                    university.Friends = university.Friends.Select(s =>
                    {
                        var facebookData = facebookFriendDatas.FirstOrDefault(f => f.Id == s.Id);
                        if (facebookData != null)
                        {
                            s.Image = facebookData.Image;
                            s.Name = facebookData.Name;
                        }
                        return s;
                    });
                }

                return JsonOk(suggestedUniversity);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Library Get friends authkey=" + authToken, ex);
                return JsonError();
            }
        }

    }
}
