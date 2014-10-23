using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries;
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
            return RedirectToRoute("Default", new { controller = "Library", Action = "Index" });
        }

        //[UserNavNWelcome]
        [HttpGet]
        [NoUniversity]
        [NoCache]
        public async Task<ActionResult> Index()
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
            if (Request.IsAjaxRequest())
            {
                return PartialView(result);
            }
            return View(result);

        }

        [HttpGet, NoUniversity, Ajax]
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
        [HttpGet, NonAjax]
        [NoCache]
        public ActionResult Choose()
        {
            return View("Empty");
        }


        [HttpGet, Ajax]
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
        

        [HttpGet, Ajax]
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

        [HttpGet, Ajax]
        public async Task<ActionResult> GetFriends(string authToken)
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
        [Ajax]
        public async Task<JsonResult> Nodes(string section)
        {
            var guid = TryParseNullableGuid(section);
            var userDetail = FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university), JsonRequestBehavior.AllowGet);
            }
            var query = new GetLibraryNodeQuery(userDetail.UniversityId.Value, guid, GetUserId());
            var result = await ZboxReadService.GetLibraryNode(query);
            return Json(new JsonResponse(true, result));

        }
        private Guid? TryParseNullableGuid(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            Guid guid;
            if (Guid.TryParse(str, out guid))
            {
                return guid;
            }
            return GuidEncoder.Decode(str);

        }

        #region DeleteNode
        [HttpPost, Ajax]
        public JsonResult DeleteNode(string id)
        {
            var guid = TryParseNullableGuid(id);
            var userDetail = FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university), JsonRequestBehavior.AllowGet);
            }
            if (!guid.HasValue)
            {
                return Json(new JsonResponse(false, "Error"));
            }

            var command = new DeleteNodeFromLibraryCommand(guid.Value, userDetail.UniversityId.Value);
            ZboxWriteService.DeleteNodeLibrary(command);
            return Json(new JsonResponse(true));

        }
        #endregion

        #region RenameNode

        [HttpGet, Ajax]
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

        [HttpPost, Ajax]
        public JsonResult RenameNode(RenameLibraryNode model)
        {
            var guid = TryParseNullableGuid(model.Id);
            if (!guid.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Error");
            }
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors().First().Value[0]));
            }

            var userDetail = FormsAuthenticationService.GetUserData();
            if (!userDetail.UniversityId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university));
            }
            try
            {
                var command = new RenameNodeCommand(model.NewName, guid.Value, userDetail.UniversityId.Value);

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
        [Ajax, HttpGet]
        public async Task<ActionResult> SelectDepartmentRussian(long universityId)
        {
            var retVal = await ZboxReadService.GetRussianDepartmentList(universityId);
            return Json(new JsonResponse(true, new { html = RenderRazorViewToString("SelectDepartment", retVal) }));
        }


        //[HttpPost, Ajax]
        //public ActionResult SelectDepartment(Guid id)
        //{
        //    var command = new SelectDepartmentCommand(id, GetUserId());
        //    ZboxWriteService.SelectDepartment(command);
        //    return Json(new JsonResponse(true));
        //}

        #region Create


        [HttpPost, Ajax]
        public ActionResult Create(CreateLibraryItem model)
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
                var parentId = TryParseNullableGuid(model.ParentId);
                var command = new AddNodeToLibraryCommand(model.Name, userDetail.UniversityId.Value, parentId, GetUserId());
                ZboxWriteService.CreateDepartment(command);
                var result = new NodeDto { Id = command.Id, Name = model.Name, Url = command.Url };
                return Json(new JsonResponse(true, result));
            }
            catch (ArgumentException ex)
            {
                TraceLog.WriteError("Library Create", ex);
                return Json(new JsonResponse(false, "unspecified error"));
            }
        }





        [HttpPost]
        [Ajax]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateBox(CreateAcademicBox model)
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
            var guid = TryParseNullableGuid(model.DepartmentId);
            if (!guid.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Departmentid is required");
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {

                var userId = GetUserId();

                var command = new CreateAcademicBoxCommand(userId, model.CourseName,
                                                           model.CourseId, model.Professor, guid.Value);
                var result = ZboxWriteService.CreateBox(command);
                return Json(new JsonResponse(true, new { result.NewBox.Url, result.NewBox.Id }));
            }
            catch (BoxNameAlreadyExistsException)
            {
                ModelState.AddModelError(string.Empty, LibraryControllerResources.course_already_exists);
                return Json(new JsonResponse(false, GetModelStateErrors()));

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("CreateAcademic user: {0} model: {1}", GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, LibraryControllerResources.Problem_with_create_a_course);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }

        [HttpGet, Ajax]
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

        //[HttpGet, Ajax]
        //[OutputCache(Duration = TimeConsts.Hour, Location = OutputCacheLocation.Any, VaryByParam = "none", VaryByCustom = CustomCacheKeys.Lang)]
        //public ActionResult CreateAcademicBoxPartial()
        //{
        //    try
        //    {
        //        return PartialView("_UploadCreateAcademicBox");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_UploadCreateAcademicBox", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}
        #endregion

        [Ajax]
        [HttpGet]
        public ActionResult NewUniversity()
        {
            return PartialView("_AddSchoolDialog", new CreateUniversity());
        }

        [HttpPost]
        [Ajax]
        public ActionResult CreateUniversity(CreateUniversity model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            //
            var command = new CreateUniversityCommand(model.Name, model.Country,
                "https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/Lib1.jpg", GetUserId());
            ZboxWriteService.CreateUniversity(command);

            FormsAuthenticationService.ChangeUniversity(command.Id);
            return Json(new JsonResponse(true, new
            {
                command.Id,
                image = command.LargeImage,
                name = model.Name
            }));
        }

        [Ajax, HttpGet]
        public ActionResult InsertCode(long universityId)
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
        [Ajax, HttpGet]
        public ActionResult InsertId(long universityId)
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



        [Ajax, HttpGet]
        public async Task<ActionResult> RussianDepartments()
        {

            var query = new GetUserMinProfileQuery(GetUserId());
            var result = await ZboxReadService.GetUserMinProfile(query);


            if (result.Score < UserController.AdminReputation)
            {
                return Json(new JsonResponse(false));
            }
            var userDetail = FormsAuthenticationService.GetUserData();
            var universityId = userDetail.UniversityId.Value;

            var retVal = await ZboxReadService.GetRussianDepartmentList(universityId);
            return Json(new JsonResponse(true, retVal));
        }

        //public async Task<JsonResult> Departments(string term)
        //{
        //    var userDetail = FormsAuthenticationService.GetUserData();
        //    if (userDetail.UniversityId == null)
        //    {
        //        return Json(new JsonResponse(false));
        //    }
        //    var universityId = userDetail.UniversityId.Value;
        //    var query = new GetDepartmentsByTermQuery(universityId, term);
        //    var retVal = await ZboxReadService.GetDepartments(query);
        //    return Json(new JsonResponse(true, retVal));
        //}

        [HttpPost, Ajax]
        public ActionResult Verify(string code)
        {
            var isValid = code == "cloudvivt";
            return Json(new JsonResponse(true, isValid));
        }
    }
}
