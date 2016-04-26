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

        public LibraryController(
            Lazy<IUniversityReadSearchProvider> universitySearch)
        {
            m_UniversitySearch = universitySearch;
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
            if (string.IsNullOrEmpty(term))
            {
                return JsonError("need term");
            }
            try
            {
                using (var source = CreateCancellationToken(cancellationToken))
                {
                    var retVal =
                        await
                            m_UniversitySearch.Value.SearchUniversityAsync(
                                new UniversitySearchQuery(term, pageNumber: page, rowsPerPage: 25), source.Token);
                    return JsonOk(retVal);
                }
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


        [HttpGet]
        public async Task<ActionResult> Nodes(string section)
        {
            try
            {
                var guid = GuidEncoder.TryParseNullableGuid(section);
                var universityId = User.GetUniversityData();

                if (!universityId.HasValue)
                {
                    return
                        JsonError(LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university);
                }
                var query = new GetLibraryNodeQuery(universityId.Value, guid, User.GetUserId());
                var result = await ZboxReadService.GetLibraryNodeAsync(query);
                return JsonOk(result);
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
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
        public JsonResult ChangeSettings(DepartmentSettings model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var guid = GuidEncoder.TryParseNullableGuid(model.Id);
            if (!guid.HasValue)
            {
                TraceLog.WriteError("need node " + model);
                return JsonError(BaseControllerResources.UnspecifiedError);
            }
            try
            {
                var command = new UpdateNodeSettingsCommand(model.Name, guid.Value, model.Settings, User.GetUserId());
                ZboxWriteService.UpdateNodeSettings(command);
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
                TraceLog.WriteError($"CreateAcademic user: {User.GetUserId()} model: {model}", ex);
                return JsonError(LibraryControllerResources.Problem_with_create_a_course);
            }
        }



        #endregion


        [HttpPost]
        public async Task<JsonResult> RequestAccess(Guid id)
        {
            if (id == Guid.Empty)
            {
                return JsonError("need dep id");
            }
            var command = new RequestAccessLibraryNodeCommand(id, User.GetUserId());
            await ZboxWriteService.RequestAccessToDepartmentAsync(command);
            return JsonOk();
        }

        [HttpPost]
        public async Task<JsonResult> ApproveRequest(Guid id, long userId)
        {
            var command = new LibraryNodeApproveAccessCommand(User.GetUserId(), id, userId);
            await ZboxWriteService.RequestAccessToDepartmentApprovedAsync(command);
            return JsonOk();

        }


        [HttpGet]
        public async Task<JsonResult> ClosedDepartment()
        {
            var retVal = await ZboxReadService.GetUserClosedDepartmentAsync(new QueryBase(User.GetUserId()));
            return JsonOk(retVal);
        }
        [HttpGet]
        public async Task<JsonResult> ClosedDepartmentMembers(Guid id)
        {
            var query = new GetClosedNodeMembersQuery(User.GetUserId(), id);
            var retVal = await ZboxReadService.GetMembersClosedDepartmendAsync(query);
            return JsonOk(retVal);
        }


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
            var claimUniversity = user.Claims.SingleOrDefault(w => w.Type == ClaimConst.UniversityIdClaim);
            var claimUniversityData = user.Claims.SingleOrDefault(w => w.Type == ClaimConst.UniversityDataClaim);

            if (claimUniversity != null)
            {
                user.RemoveClaim(claimUniversity);
            }
            if (claimUniversityData != null)
            {
                user.RemoveClaim(claimUniversityData);
            }

            user.AddClaim(new Claim(ClaimConst.UniversityIdClaim,
                    command.Id.ToString(CultureInfo.InvariantCulture)));

            user.AddClaim(new Claim(ClaimConst.UniversityDataClaim,
                    command.Id.ToString(CultureInfo.InvariantCulture)));


            AuthenticationManager.SignIn(user);

            return JsonOk(new
            {
                command.Id,
            });
        }
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
    }
}
