using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Cloudents.Mvc4WebRole.Extensions;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [ZboxAuthorize]
    [NoUniversity]
    public class SearchController : BaseController
    {
        public SearchController(IZboxReadService zboxReadService, IZboxWriteService zboxWriteService, IFormsAuthenticationService formAuthService)
            : base(zboxWriteService, zboxReadService, formAuthService)
        {
        }
        //
        // GET: /Search/
        [UserNavNWelcome]
        [NonAjax]
        [HttpGet]
        [CompressFilter]
        public ActionResult Index(string q)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [Ajax]
        public async Task<ActionResult> MinQuery(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return new EmptyResult();
            }
            var userDetail = m_FormsAuthenticationService.GetUserData();
            var query = new GroupSearchQuery(q, userDetail.UniversityId.Value, GetUserId());
            var result = await m_ZboxReadService.Search(query);

            return this.CdJson(result);

        }
    }
}