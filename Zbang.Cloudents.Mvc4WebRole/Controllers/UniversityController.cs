using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using DevTrends.MvcDonutCaching;
using Microsoft.Owin.Security;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Dashboard;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    public class UniversityController : BaseController
    {
        private readonly Lazy<IUniversityReadSearchProvider> m_UniversitySearch;

        public UniversityController(
            Lazy<IUniversityReadSearchProvider> universitySearch)
        {
            m_UniversitySearch = universitySearch;
        }



        [DonutOutputCache(CacheProfile = "FullPage")]
        [NoUniversity, ActionName("Index")]
        public async Task<ActionResult> IndexAsync(long universityId, string universityName)
        {
            var query = new UniversityQuery(universityId);
            var model = await ZboxReadService.GetUniversityInfoAsync(query);
            if (UrlConst.NameToQueryString(model.Name) != universityName)
            {
                return RedirectToRoutePermanent("universityLibraryAuth", new RouteValueDictionary
                {
                    ["universityId"] = model.Id,
                    ["universityName"] = UrlConst.NameToQueryString(model.Name)
                });
            }

            return View("Empty");
        }


        [NoUniversity, HttpGet]
        [Route("library")]
        public async Task<RedirectToRouteResult> LibraryRedirectAsync()
        {

            // ReSharper disable once PossibleInvalidOperationException
            var universityWrapper = User.GetUniversityId().Value;

            var query = new UniversityQuery(universityWrapper);
            var model = await ZboxReadService.GetUniversityInfoAsync(query);

            return RedirectToRoutePermanent("universityLibrary", new RouteValueDictionary
            {
                ["universityId"] = model.Id,
                ["universityName"] = UrlConst.NameToQueryString(model.Name)
            });
        }

        [Route("library/choose")]
        public ActionResult LibraryChooseRedirect()
        {
            return RedirectToAction("choose");
        }



        [NoUniversity, HttpGet]
        [Route("library/{LibId}/{LibName}")]
        public async Task<RedirectToRouteResult> LibraryRedirectWithNodeAsync(string libId, string libName)
        {

            // ReSharper disable once PossibleInvalidOperationException
            var universityWrapper = User.GetUniversityId().Value;

            var query = new UniversityQuery(universityWrapper);
            var model = await ZboxReadService.GetUniversityInfoAsync(query);

            return RedirectToRoutePermanent("universityLibraryNodes", new RouteValueDictionary
            {
                ["universityId"] = model.Id,
                ["universityName"] = UrlConst.NameToQueryString(model.Name),
                ["libraryid"] = libId,
                ["libraryname"] = libName
            });
        }


        [HttpGet, NoUniversity(Order = 1)]
        [DonutOutputCache(CacheProfile = "PartialPage", Order = 2)]
        public ActionResult IndexPartial()
        {
            return PartialView("Index2");
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "FullPage")]
        public ActionResult Choose()
        {
            return View("Empty");
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult ClassChoosePartial()
        {
            return View("ClassChoose", false);
        }
        public ActionResult ClassChooseMobilePartial()
        {
            return PartialView("ClassChoose", true);
        }


        [HttpGet, ActionName("ChoosePartial")]
        public async Task<PartialViewResult> ChoosePartialAsync()
        {
            var userIp = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(userIp))
            {
                userIp = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (HttpContext.Request.IsLocal)
            {
                userIp = "81.218.135.73";
            }
            var command = new AddUserLocationActivityCommand(userIp, User.GetUserId(), HttpContext.Request.UserAgent);
            await ZboxWriteService.AddUserLocationActivityAsync(command);
            //TODO: remove that
            ViewBag.country = command.Country;// await GetCountryByIpAsync(HttpContext);
            return PartialView("Choose");
        }

        [Route("library/unisettings")]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult UniSettings()
        {
            return PartialView("_Settings");
        }


        [HttpGet, ActionName("Search")]
        public async Task<JsonResult> SearchAsync(string term, int page, CancellationToken cancellationToken)
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

        //private Task<string> GetCountryByIpAsync(/*HttpContextBase context*/)
        //{
        //    string userIp = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (string.IsNullOrWhiteSpace(userIp))
        //    {
        //        userIp = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
        //    }
        //    if (HttpContext.Request.IsLocal)
        //    {
        //        userIp = "81.218.135.73";
        //    }
        //    var ipNumber = Ip2Long(userIp);
        //    return ZboxReadService.GetLocationByIpAsync(new GetCountryByIpQuery(ipNumber));
        //}

        //private static long Ip2Long(string ip)
        //{
        //    double num = 0;
        //    if (!string.IsNullOrEmpty(ip))
        //    {
        //        string[] ipBytes = ip.Split('.');
        //        for (int i = ipBytes.Length - 1; i >= 0; i--)
        //        {
        //            num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
        //        }
        //    }
        //    return (long)num;
        //}


        [HttpGet, ActionName("Nodes")]
        public async Task<ActionResult> NodesAsync(string section, long universityId/*, bool? skipUrl*/)
        {
            try
            {
                if (Request.UrlReferrer == null)
                {
                    TraceLog.WriteError("need url Referrer");
                    return JsonError();

                }
                var guid = GuidEncoder.TryParseNullableGuid(section);
                var query = new GetLibraryNodeQuery(universityId, guid, User.GetUserId());
                var result = await ZboxReadService.GetLibraryNodeAsync(query);
                if (result.Nodes == null) return JsonOk(result);
                var route = BuildRouteDataFromUrl(Request.UrlReferrer.AbsoluteUri);

                var universityName = route.Values["universityName"];
                var universityNameDecoded = "t";
                if (universityName != null)
                {
                    universityNameDecoded = HttpUtility.UrlDecode(universityName.ToString());
                }
                foreach (var node in result.Nodes)
                {
                    node.Url = Url.RouteUrlCache("universityLibraryNodes", new RouteValueDictionary
                    {
                        ["universityId"] = universityId,
                        ["universityName"] = universityNameDecoded,
                        ["id"] = GuidEncoder.Encode(node.Id),
                        ["libraryname"] = UrlConst.NameToQueryString(node.Name)
                    });
                }
                if (result.Details == null) return JsonOk(result);
                if (result.Details.ParentId.HasValue)
                {
                    result.Details.ParentUrl = Url.RouteUrlCache("universityLibraryNodes", new RouteValueDictionary
                    {
                        ["universityId"] = universityId,
                        ["universityName"] = universityNameDecoded,
                        ["id"] = GuidEncoder.Encode(result.Details.ParentId.Value),
                        ["libraryname"] = UrlConst.NameToQueryString(result.Details.ParentName)
                    });
                }
                else
                {
                    result.Details.ParentUrl = Url.RouteUrlCache("universityLibrary", new RouteValueDictionary
                    {
                        ["universityId"] = universityId,
                        ["universityName"] = universityNameDecoded
                    });
                }
                return JsonOk(result);
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
        }


        #region DeleteNode
        [HttpPost, ActionName("DeleteNode")]
        public async Task<JsonResult> DeleteNodeAsync(string id)
        {
            var guid = GuidEncoder.TryParseNullableGuid(id);

            var universityId = User.GetUniversityDataId();

            if (!universityId.HasValue)
            {
                return JsonError(LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university);
            }
            if (!guid.HasValue)
            {
                return JsonError("Error");
            }

            var command = new DeleteNodeFromLibraryCommand(guid.Value, universityId.Value);
            await ZboxWriteService.DeleteNodeLibraryAsync(command);
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


        [HttpPost, ActionName("Create")]
        public async Task<JsonResult> CreateAsync(CreateLibraryItem model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var universityId = User.GetUniversityDataId();

            if (!universityId.HasValue)
            {
                return JsonError(LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university);
            }
            if (Request.UrlReferrer == null)
            {
                TraceLog.WriteError("need url Referrer");
                return JsonError();

            }

            try
            {
                var parentId = GuidEncoder.TryParseNullableGuid(model.ParentId);
                var command = new AddNodeToLibraryCommand(model.Name, universityId.Value, parentId, User.GetUserId());
                await ZboxWriteService.CreateDepartmentAsync(command);
                var result = new NodeDto
                {
                    Id = command.Id,
                    Name = model.Name
                };
                if (!model.SkipUrl)
                {
                    var route = BuildRouteDataFromUrl(Request.UrlReferrer.AbsoluteUri);

                    var universityName = route.Values["universityName"];
                    var url = Url.RouteUrlCache("universityLibraryNodes", new RouteValueDictionary
                    {
                        ["universityId"] = universityId,
                        ["universityName"] = HttpUtility.UrlDecode(universityName.ToString()),
                        ["id"] = GuidEncoder.Encode(command.Id),
                        ["libraryname"] = UrlConst.NameToQueryString(model.Name)
                    });
                    result.Url = url;
                }
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





        [HttpPost, ActionName("CreateBox")]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateBoxAsync(CreateAcademicBox model)
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
                                                           model.CourseId, model.Professor, guid.Value, universityId.Value);
                var result = await ZboxWriteService.CreateBoxAsync(command);
                return JsonOk(new { result.Url, result.Id });
            }
            catch (BoxNameAlreadyExistsException)
            {
                return JsonError(BoxControllerResources.BoxExists);

            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"CreateAcademic user: {User.GetUserId()} model: {model}", ex);
                return JsonError(LibraryControllerResources.Problem_with_create_a_course);
            }
        }



        #endregion


        [HttpPost, ActionName("RequestAccess")]
        public async Task<JsonResult> RequestAccessAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return JsonError("need dep id");
            }
            var command = new RequestAccessLibraryNodeCommand(id, User.GetUserId());
            await ZboxWriteService.RequestAccessToDepartmentAsync(command);
            return JsonOk();
        }

        [HttpPost, ActionName("ApproveRequest")]
        public async Task<JsonResult> ApproveRequestAsync(Guid id, long userId)
        {
            var command = new LibraryNodeApproveAccessCommand(User.GetUserId(), id, userId);
            await ZboxWriteService.RequestAccessToDepartmentApprovedAsync(command);
            return JsonOk();

        }


        [HttpGet, ActionName("ClosedDepartment")]
        public async Task<JsonResult> ClosedDepartmentAsync()
        {
            var retVal = await ZboxReadService.GetUserClosedDepartmentAsync(new QueryBase(User.GetUserId()));
            return JsonOk(retVal);
        }
        [HttpGet, ActionName("ClosedDepartmentMembers")]
        public async Task<JsonResult> ClosedDepartmentMembersAsync(Guid id)
        {
            var query = new GetClosedNodeMembersQuery(User.GetUserId(), id);
            var retVal = await ZboxReadService.GetMembersClosedDepartmendAsync(query);
            return JsonOk(retVal);
        }


        [HttpPost, ActionName("CreateUniversity")]
        public async Task<JsonResult> CreateUniversityAsync(CreateUniversity model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var command = new CreateUniversityCommand(model.Name, model.Country, User.GetUserId());
            await ZboxWriteService.CreateUniversityAsync(command);


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
                url = Url.RouteUrlCache("universityLibrary", new RouteValueDictionary
                {
                    ["universityId"] = command.Id,
                    ["universityName"] = HttpUtility.UrlDecode(model.Name)
                })//,
                //command.Id,
            });
        }

        [HttpGet, ActionName("AllNodes")]
        public async Task<JsonResult> AllNodesAsync()
        {
            var universityId = User.GetUniversityId();
            if (!universityId.HasValue)
            {
                return JsonError(LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university);
            }
            var retVal = await ZboxReadService.GetUniversityNodesAsync(universityId.Value);
            return JsonOk(retVal);
        }
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
    }
}
