using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mobile.Extensions;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    [NoUniversity]
    public class DashboardController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        [DonutOutputCache(CacheProfile = "PartialPage",
           Options = OutputCacheOptions.IgnoreQueryString
           )]
        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }

        [HttpGet]
        public async Task<JsonResult> BoxList(int page)
        {
            System.Threading.Thread.Sleep(5000);
            try
            {
                var query = new GetBoxesQuery(User.GetUserId(), page, 15);
                var data = await ZboxReadService.GetUserBoxes(query);

                return JsonOk(data.Select(s => new
                {
                    s.Name,
                    s.Url,
                    s.ItemCount,
                    s.CommentCount
                }));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("BoxList user: {0}", User.GetUserId()), ex);
                return JsonError();
            }
        }

        [HttpGet]
        public async Task<JsonResult> RecommendedCourses()
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            // ReSharper disable once PossibleInvalidOperationException - universityid have value because no university attribute
            var universityWrapper = userDetail.UniversityDataId.Value;

            var query = new RecommendedCoursesQuery(universityWrapper, User.GetUserId());
            var result = await ZboxReadService.GetRecommendedCourses(query);
            return JsonOk(result.Select(s => new
            {
                s.Url,
                s.Professor,
                s.CourseCode,
                s.Name,
                s.MembersCount,
                s.ItemCount

            }));
        }
    }
}
