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
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Dto.Library;
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
            //var userDetail = FormsAuthenticationService.GetUserData();
            //var universityId = User.GetUniversityId();
            //if (!universityId.HasValue)
            //{
            //    return RedirectToAction("Choose");
            //}
            //var query = new GetUniversityDetailQuery(universityId.Value
            //     );

            //var result = await ZboxReadService.GetUniversityDetail(query);
            //if (result.Id == 0)
            //{
            //    return RedirectToAction("Choose");
            //}
            //return PartialView("Index2", result);
            return PartialView("Index2");
        }

        //TODO: put output cache
        [HttpGet]
        [NoCache]
        [RedirectToMobile(Order = 1)]
        public ActionResult Choose()
        {
            return View("Empty");
        }


        [HttpGet]
        public async Task<PartialViewResult> ChoosePartial()
        {

            ViewBag.country = await UserLanguage.GetCountryByIpAsync(HttpContext); ;
            return PartialView("Choose");
        }


        [HttpGet]
        public async Task<JsonResult> SearchUniversity(string term, CancellationToken cancellationToken)
        {
            CancellationToken disconnectedToken = Response.ClientDisconnectedToken;
            var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, disconnectedToken);
            var country = string.Empty;
            if (string.IsNullOrEmpty(term))
            {
                country = await UserLanguage.GetCountryByIpAsync(HttpContext);
            }
            try
            {
                var retVal = await m_UniversitySearch.Value.SearchUniversityAsync(new UniversitySearchQuery(term, country: country, rowsPerPage: 25), source.Token);
                return JsonOk(retVal);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("SeachUniversity term:  " + term, ex);
                return JsonError();
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUniversityByFriends(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return JsonError();
            }
            try
            {
                var friendsId = await m_FacebookService.Value.GetFacebookUserFriends(token);
                var facebookFriendData = friendsId as FacebookFriendData[] ?? friendsId.ToArray();
                var suggestedUniversity =
                    await ZboxReadService.GetUniversityListByFriendsIdsAsync(facebookFriendData.Select(s => s.Id));

                //foreach (var university in suggestedUniversity)
                //{
                //    university.UserImages = university.Friends.Select(s =>
                //    {
                //        var facebookData = facebookFriendData.FirstOrDefault(f => f.Id == s.Id);
                //        if (facebookData != null)
                //        {
                //            s.Image = facebookData.Image;
                //            s.Name = facebookData.Name;
                //        }
                //        return s;
                //    });
                //}

                return JsonOk(suggestedUniversity);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Library Get friends authentication key=" + token, ex);
                return JsonError();
            }
        }



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
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university), JsonRequestBehavior.AllowGet);
            }
            if (!guid.HasValue)
            {
                return Json(new JsonResponse(false, "Error"));
            }

            var command = new DeleteNodeFromLibraryCommand(guid.Value, universityId.Value);
            ZboxWriteService.DeleteNodeLibrary(command);
            return Json(new JsonResponse(true));

        }
        #endregion

        #region RenameNode

        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]

        //public ActionResult Rename()
        //{
        //    try
        //    {
        //        return PartialView("Rename");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("Rename ", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}

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
            catch (ArgumentException ex)
            {
                return JsonError(ex.Message);
            }
        }
        #endregion

        //[ActionName("SelectDepartment")]
        //[HttpGet]
        //public async Task<JsonResult> SelectDepartmentRussian(long universityId)
        //{
        //    var retVal = await ZboxReadService.GetRussianDepartmentList(universityId);
        //    return Json(new JsonResponse(true, new { html = RenderRazorViewToString("SelectDepartment", retVal) }));
        //}



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
                return JsonError("Cannot add library to box node");
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

            user.RemoveClaim(claimUniversity);
            user.RemoveClaim(claimUniversityData);


            user.AddClaim(new Claim(ClaimConsts.UniversityIdClaim,
                    command.Id.ToString(CultureInfo.InvariantCulture)));

            user.AddClaim(new Claim(ClaimConsts.UniversityDataClaim,
                    command.Id.ToString(CultureInfo.InvariantCulture)));


            AuthenticationManager.SignIn(user);

            return JsonOk(new
            {
                command.Id,
                //name = model.Name
            });
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        //[HttpGet]
        //public JsonResult InsertId(long universityId)
        //{
        //    dynamic universityData = TempData["universityText"];
        //    if (universityData != null)
        //    {
        //        ViewBag.TextPopupUpper = universityData["TextPopupUpper"];
        //        ViewBag.TextPopupLower = universityData["TextPopupLower"];
        //    }
        //    return Json(new JsonResponse(true, new { html = RenderRazorViewToString("InsertID", new Models.Account.Settings.University()) }));
        //}



        //[HttpGet]
        //public async Task<JsonResult> RussianDepartments()
        //{
        //    var universityId = User.GetUniversityId().Value;

        //    var retVal = await ZboxReadService.GetRussianDepartmentList(universityId);
        //    return Json(new JsonResponse(true, retVal));
        //}



        //[HttpPost]
        //public JsonResult Verify(string code)
        //{
        //    var isValid = code == "cloudvivt";
        //    return Json(new JsonResponse(true, isValid));
        //}
    }
}
