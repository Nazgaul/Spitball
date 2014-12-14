using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Library;

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
        public async Task<ActionResult> IndexPartial()
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            if (userDetail == null || !userDetail.UniversityId.HasValue)
            {
                return RedirectToAction("Choose");
            }
            var query = new GetUniversityDetailQuery(userDetail.UniversityId.Value
                );

            var result = await ZboxReadService.GetUniversityDetail(query);
            if (result.Id == 0)
            {
                return RedirectToAction("Choose");
            }
            return PartialView("Index", result);
        }

        //TODO: put output cache
        [HttpGet]
        [NoCache]
        public ActionResult Choose()
        {
            return View("Empty");
        }


        [HttpGet]
        public PartialViewResult ChoosePartial()
        {
            var country = UserLanguage.GetCountryByIp(HttpContext);
            var haveUniversity = false;
            var userData = FormsAuthenticationService.GetUserData();
            if (userData != null && userData.UniversityId.HasValue)
            {
                haveUniversity = true;
            }

            ViewBag.country = country;
            ViewBag.haveUniversity = haveUniversity.ToString().ToLower();

            return PartialView("_SelectUni");
        }
        

        [HttpGet]
        public async Task<JsonResult> SearchUniversity(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new JsonResponse(false));
            }
            try
            {
                var retVal = await m_UniversitySearch.Value.SearchUniversity(term);
                return Json(new JsonResponse(true, retVal));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("SeachUniversity term:  " + term, ex);
                return Json(new JsonResponse(false));
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

                return Json(new JsonResponse(true, suggestedUniversity));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Library Get friends authkey=" + authToken, ex);
                return Json(new JsonResponse(false));
            }
        }



        [HttpGet]
        public async Task<JsonResult> Nodes(string section)
        {
            var guid = GuidEncoder.TryParseNullableGuid(section);
            var userDetail = FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityDataId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university), JsonRequestBehavior.AllowGet);
            }
            var query = new GetLibraryNodeQuery(userDetail.UniversityDataId.Value, guid, User.GetUserId());
            var result = await ZboxReadService.GetLibraryNode(query);
            return Json(new JsonResponse(true, result));

        }
        

        #region DeleteNode
        [HttpPost]
        public JsonResult DeleteNode(string id)
        {
            var guid = GuidEncoder.TryParseNullableGuid(id);
            var userDetail = FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityDataId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university), JsonRequestBehavior.AllowGet);
            }
            if (!guid.HasValue)
            {
                return Json(new JsonResponse(false, "Error"));
            }

            var command = new DeleteNodeFromLibraryCommand(guid.Value, userDetail.UniversityDataId.Value);
            ZboxWriteService.DeleteNodeLibrary(command);
            return Json(new JsonResponse(true));

        }
        #endregion

        #region RenameNode

        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]

        public ActionResult Rename()
        {
            try
            {
                return PartialView("Rename");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Rename ", ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpPost]
        public JsonResult RenameNode(RenameLibraryNode model)
        {
            var guid = GuidEncoder.TryParseNullableGuid(model.Id);
            if (!guid.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Error");
            }
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors().First().Value[0]));
            }

            var userDetail = FormsAuthenticationService.GetUserData();
            if (!userDetail.UniversityDataId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university));
            }
            try
            {
                var command = new RenameNodeCommand(model.NewName, guid.Value, userDetail.UniversityDataId.Value);

                ZboxWriteService.RenameNodeLibrary(command);
                return Json(new JsonResponse(true));
            }
            catch (ArgumentException ex)
            {
                return Json(new JsonResponse(false, ex.Message));
            }
        }
        #endregion

        [ActionName("SelectDepartment")]
        [HttpGet]
        public async Task<JsonResult> SelectDepartmentRussian(long universityId)
        {
            var retVal = await ZboxReadService.GetRussianDepartmentList(universityId);
            return Json(new JsonResponse(true, new { html = RenderRazorViewToString("SelectDepartment", retVal) }));
        }



        #region Create


        [HttpPost]
        public JsonResult Create(CreateLibraryItem model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userDetail = FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university));
            }

            try
            {
                var parentId = GuidEncoder.TryParseNullableGuid(model.ParentId);
                var command = new AddNodeToLibraryCommand(model.Name, userDetail.UniversityId.Value, parentId, User.GetUserId());
                ZboxWriteService.CreateDepartment(command);
                var result = new NodeDto { Id = command.Id, Name = model.Name, Url = command.Url };
                return Json(new JsonResponse(true, result));
            }
            catch (ArgumentException ex)
            {
                TraceLog.WriteError("Library Create", ex);
                return Json(new JsonResponse(false, ex.Message));
            }
        }





        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult CreateBox(CreateAcademicBox model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userDetail = FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                ModelState.AddModelError(string.Empty, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var guid = GuidEncoder.TryParseNullableGuid(model.DepartmentId);
            if (!guid.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Department id is required");
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {

                var userId = User.GetUserId();

                var command = new CreateAcademicBoxCommand(userId, model.CourseName,
                                                           model.CourseId, model.Professor, guid.Value);
                var result = ZboxWriteService.CreateBox(command);
                return Json(new JsonResponse(true, new { result.Url, result.Id }));
            }
            catch (BoxNameAlreadyExistsException)
            {
                ModelState.AddModelError(string.Empty, LibraryControllerResources.course_already_exists);
                return Json(new JsonResponse(false, GetModelStateErrors()));

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("CreateAcademic user: {0} model: {1}", User.GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, LibraryControllerResources.Problem_with_create_a_course);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }

        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]

        public ActionResult CreateDepartmentPartial()
        {
            try
            {
                return PartialView("_CreateLibraryItem");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_CreateLibraryItem", ex);
                return Json(new JsonResponse(false));
            }
        }

       
        #endregion

        [HttpGet]
        public PartialViewResult NewUniversity()
        {
            return PartialView("_AddSchoolDialog", new CreateUniversity());
        }

        [HttpPost]
        
        public JsonResult CreateUniversity(CreateUniversity model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            //
            var command = new CreateUniversityCommand(model.Name, model.Country,
                "https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/Lib1.jpg", User.GetUserId());
            ZboxWriteService.CreateUniversity(command);

            FormsAuthenticationService.ChangeUniversity(command.Id, command.Id);
            return Json(new JsonResponse(true, new
            {
                command.Id,
                image = command.LargeImage,
                name = model.Name
            }));
        }

        [HttpGet]
        public JsonResult InsertCode(long universityId)
        {
            // var userData = m_UserProfile.Value.GetUserData(ControllerContext);
            switch (universityId)
            {
                case 19878:
                    ViewBag.AgudaName = "מכללת אשקלון - היחידה ללימודי חוץ";
                    ViewBag.AgudaMail = "shivok@ash-college.ac.il";

                    ViewBag.AgudaPhone = "או צרו קשר ישירות עם מזכירות האגודה בטלפון\n08-6789250.";
                    break;
                case 1173:
                    ViewBag.AgudaName = "אגודת הסטודנטים בסימינר הקיבוצים";
                    ViewBag.AgudaMail = "maagar.aguda@gmail.com";
                    ViewBag.AgudaPhone = string.Empty;
                    break;
                case 22906:
                    ViewBag.AgudaName = "תות תקשורת ותוצאות";
                    ViewBag.AgudaMail = "kipigilad@walla.com‏";

                    ViewBag.AgudaPhone = "או צרו קשר ישירות עם מזכירות האגודה בטלפון\n054-3503509.";
                    break;
                case 64805:
                    ViewBag.AgudaName = string.Empty;
                    ViewBag.AgudaMail = string.Empty;

                    ViewBag.AgudaPhone = string.Empty;
                    break;
                default:
                    ViewBag.AgudaName = "המרכז ללימודים אקדמיים אור יהודה";
                    ViewBag.AgudaMail = "aguda@mla.ac.il";
                    ViewBag.AgudaPhone = "או צרו קשר ישירות עם מזכירות האגודה בטלפון\n072-2220692.";
                    break;
            }
            // ViewBag.userName = userData.Name;
            return Json(new JsonResponse(true, new { html = RenderRazorViewToString("InsertCode", new Models.Account.Settings.University { UniversityId = universityId }) }));
        }
        [HttpGet]
        public JsonResult InsertId(long universityId)
        {
            // var userData = m_UserProfile.Value.GetUserData(ControllerContext);
            switch (universityId)
            {
                default:
                    ViewBag.AgudaSentence = "המאגר האקדמי של המכללה למינהל פתוח לכל חברי אגודת הסטודנטים של המכללה למינהל.  אימות חברי אגודה ע\"י מספר ת\"ז";
                    //ViewBag.AgudaName = "המרכז ללימודים אקדמיים אור יהודה";
                    ViewBag.AgudaMail = "aguda4u.co.il@gmail.com";
                    ViewBag.AgudaPhone = "או צרו קשר ישירות עם מזכירות האגודה בטלפון 03-9628930";
                    break;
            }
            //  ViewBag.userName = userData.Name;
            return Json(new JsonResponse(true, new { html = RenderRazorViewToString("InsertID", null) }));
        }



        [HttpGet]
        public async Task<JsonResult> RussianDepartments()
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            var universityId = userDetail.UniversityId.Value;

            var retVal = await ZboxReadService.GetRussianDepartmentList(universityId);
            return Json(new JsonResponse(true, retVal));
        }

        

        [HttpPost]
        public JsonResult Verify(string code)
        {
            var isValid = code == "cloudvivt";
            return Json(new JsonResponse(true, isValid));
        }
    }
}
