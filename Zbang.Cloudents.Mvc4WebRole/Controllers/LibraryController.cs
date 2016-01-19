using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Microsoft.Owin.Security;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
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

        public ActionResult DepartmentRedirect()
        {
            return RedirectToRoute("LibraryDesktop");
        }


        [HttpGet, NoUniversity]
        public ActionResult IndexPartial()
        {
            return PartialView("Index2");
        }

        //TODO: put output cache
        [HttpGet]
        [NoCache]
        public ActionResult Choose()
        {
            return View("Empty");
        }


        [HttpGet]
        public async Task<PartialViewResult> ChoosePartial()
        {

            ViewBag.country = await GetCountryByIpAsync(HttpContext);
            return PartialView("Choose");
        }


        [HttpGet]
        public async Task<JsonResult> SearchUniversity(string term, int page, CancellationToken cancellationToken)
        {
            var source = CreateCancellationToken(cancellationToken);
            var country = string.Empty;
            if (string.IsNullOrEmpty(term))
            {
                country = await GetCountryByIpAsync(HttpContext);
            }
            try
            {
                var retVal = await m_UniversitySearch.Value.SearchUniversityAsync(new UniversitySearchQuery(term, pageNumber: page, country: country, rowsPerPage: 25), source.Token);
                return JsonOk(retVal);
            }
            catch (OperationCanceledException)
            {
                TraceLog.WriteInfo("search university - abort");
                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("SeachUniversity term:  " + term, ex);
                return JsonError();
            }
        }

        private Task<string> GetCountryByIpAsync(HttpContextBase context)
        {
            string userIp = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(userIp))
            {
                userIp = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (context.Request.IsLocal)
            {
                userIp = "81.218.135.73";
            }
            var ipNumber = Ip2Long(userIp);
            return ZboxReadService.GetLocationByIpAsync(new GetCountryByIpQuery(ipNumber));
        }

        private static long Ip2Long(string ip)
        {
            double num = 0;
            if (!string.IsNullOrEmpty(ip))
            {
                string[] ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }
            }
            return (long)num;
        }

        //[HttpGet]
        //public async Task<JsonResult> GetUniversityByFriends(string token)
        //{
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return JsonError();
        //    }
        //    try
        //    {
        //        var friendsId = await m_FacebookService.Value.GetFacebookUserFriends(token);
        //        var facebookFriendData = friendsId as FacebookFriendData[] ?? friendsId.ToArray();
        //        var suggestedUniversity =
        //            await ZboxReadService.GetUniversityListByFriendsIdsAsync(facebookFriendData.Select(s => s.Id));


        //        return JsonOk(suggestedUniversity);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("Library Get friends authentication key=" + token, ex);
        //        return JsonError();
        //    }
        //}



        [HttpGet]
        public async Task<JsonResult> Nodes(string section)
        {
            var guid = GuidEncoder.TryParseNullableGuid(section);
            var universityId = User.GetUniversityData();

            if (!universityId.HasValue)
            {
                return JsonError(LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university);
            }
            var query = new GetLibraryNodeQuery(universityId.Value, guid, User.GetUserId());
            var result = await ZboxReadService.GetLibraryNodeAsync(query);
            return JsonOk(result);

        }


        #region DeleteNode
        [HttpPost]
        public JsonResult DeleteNode(string id)
        {
            var guid = GuidEncoder.TryParseNullableGuid(id);

            var universityId = User.GetUniversityData();

            if (!universityId.HasValue)
            {
                return JsonError(LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university);
            }
            if (!guid.HasValue)
            {
                return JsonError("Error");
            }

            var command = new DeleteNodeFromLibraryCommand(guid.Value, universityId.Value);
            ZboxWriteService.DeleteNodeLibrary(command);
            return JsonOk();

        }
        #endregion

        #region RenameNode



        [HttpPost]
        public JsonResult RenameNode(RenameLibraryNode model)
        {
            var guid = GuidEncoder.TryParseNullableGuid(model.Id);
            if (!guid.HasValue)
            {
                return JsonError("need node");
            }
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }

            var universityId = User.GetUniversityData();

            if (!universityId.HasValue)
            {
                return JsonError(LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university);
            }
            try
            {
                var command = new RenameNodeCommand(model.NewName, guid.Value, universityId.Value);

                ZboxWriteService.RenameNodeLibrary(command);
                return JsonOk();
            }
            catch (DuplicateDepartmentNameException)
            {
                return JsonError(LibraryControllerResources.DepartmentAlreadyExists);
            }
        }
        #endregion


        #region Create


        [HttpPost]
        public JsonResult Create(CreateLibraryItem model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var universityId = User.GetUniversityData();

            if (!universityId.HasValue)
            {
                return JsonError(LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university);
            }

            try
            {
                var parentId = GuidEncoder.TryParseNullableGuid(model.ParentId);
                var command = new AddNodeToLibraryCommand(model.Name, universityId.Value, parentId, User.GetUserId());
                ZboxWriteService.CreateDepartment(command);
                var result = new NodeDto { Id = command.Id, Name = model.Name, Url = command.Url };
                return JsonOk(result);
            }
            catch (DuplicateDepartmentNameException)
            {
                return JsonError(LibraryControllerResources.DepartmentAlreadyExists);
            }
            catch (BoxesInDepartmentNodeException)
            {
                return JsonError(LibraryControllerResources.LibraryController_Create_Cannot_add_library_to_box_node);
            }
        }





        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult CreateBox(CreateAcademicBox model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var universityId = User.GetUniversityId();

            if (!universityId.HasValue)
            {
                return JsonError(LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university);
            }
            var guid = GuidEncoder.TryParseNullableGuid(model.DepartmentId);
            if (!guid.HasValue)
            {
                return JsonError(LibraryControllerResources.LibraryController_CreateBox_Department_id_is_required);
            }
            try
            {

                var userId = User.GetUserId();

                var command = new CreateAcademicBoxCommand(userId, model.CourseName,
                                                           model.CourseId, model.Professor, guid.Value);
                var result = ZboxWriteService.CreateBox(command);
                return JsonOk(new { result.Url });
            }
            catch (BoxNameAlreadyExistsException)
            {
                return JsonError(LibraryControllerResources.course_already_exists);

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("CreateAcademic user: {0} model: {1}", User.GetUserId(), model), ex);
                return JsonError(LibraryControllerResources.Problem_with_create_a_course);
            }
        }

        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]

        //public ActionResult CreateDepartmentPartial()
        //{
        //    try
        //    {
        //        return PartialView("_CreateLibraryItem");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_CreateLibraryItem", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}


        #endregion

        //[HttpGet]
        //public PartialViewResult NewUniversity()
        //{
        //    return PartialView("_AddSchoolDialog", new CreateUniversity());
        //}

        [HttpPost]

        public JsonResult CreateUniversity(CreateUniversity model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var command = new CreateUniversityCommand(model.Name, model.Country, User.GetUserId());
            ZboxWriteService.CreateUniversity(command);


            var user = (ClaimsIdentity)User.Identity;
            var claimUniversity = user.Claims.SingleOrDefault(w => w.Type == ClaimConsts.UniversityIdClaim);
            var claimUniversityData = user.Claims.SingleOrDefault(w => w.Type == ClaimConsts.UniversityDataClaim);

            if (claimUniversity != null)
            {
                user.RemoveClaim(claimUniversity);
            }
            if (claimUniversityData != null)
            {
                user.RemoveClaim(claimUniversityData);
            }

            user.AddClaim(new Claim(ClaimConsts.UniversityIdClaim,
                    command.Id.ToString(CultureInfo.InvariantCulture)));

            user.AddClaim(new Claim(ClaimConsts.UniversityDataClaim,
                    command.Id.ToString(CultureInfo.InvariantCulture)));


            AuthenticationManager.SignIn(user);

            return JsonOk(new
            {
                command.Id,
            });
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

    }
}
