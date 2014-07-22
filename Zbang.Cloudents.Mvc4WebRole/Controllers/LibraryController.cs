using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.Library;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Library;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    public class LibraryController : BaseController
    {
        private readonly Lazy<ITableProvider> m_TableProvider;
        private readonly Lazy<IUserProfile> m_UserProfile;
        private readonly Lazy<IIdGenerator> m_IdGenerator;
        private readonly Lazy<IUniversityReadSearchProvider> m_UniversitySearch;
        private readonly Lazy<IFacebookService> m_FacebookService;

        public LibraryController(
            Lazy<ITableProvider> tableProvider,
            Lazy<IUserProfile> userProfile,
            Lazy<IIdGenerator> idGenerator,
            Lazy<IUniversityReadSearchProvider> universitySearch,
            Lazy<IFacebookService> facebookService)
           
        {
            m_TableProvider = tableProvider;
            m_UserProfile = userProfile;
            m_IdGenerator = idGenerator;
            m_UniversitySearch = universitySearch;
            m_FacebookService = facebookService;
        }


        [UserNavNWelcome]
        [HttpGet]
        //[AjaxCache(TimeConsts.Minute * 10)]
        [NoUniversity]
        [CacheFilter]
        public async Task<ActionResult> Index(Guid? libId)
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            if (userDetail == null || !userDetail.UniversityId.HasValue)
            {
                return RedirectToAction("Choose");
            }

            var universityWrapper = userDetail.UniversityWrapperId ?? userDetail.UniversityId.Value;

            var query = new GetUniversityDetailQuery(userDetail.UniversityId.Value,
                universityWrapper);

            var result = await ZboxReadService.GetUniversityDetail(query);
            if (result.Id == 0)
            {
                return RedirectToAction("Choose");
            }
            //TODO: bring with one roundtrip
            var queryNodes = new GetLibraryNodeQuery(userDetail.UniversityId.Value, libId, GetUserId(), 0, OrderBy.LastModified);
            var data = ZboxReadService.GetLibraryNode(queryNodes);
            var serializer = new JsonNetSerializer();

            ViewBag.data = serializer.Serialize(data);

            if (Request.IsAjaxRequest())
            {
                return PartialView(result);
            }
            return View(result);

        }

        

        [HttpGet]
        public ActionResult Choose()
        {
            var country = GetUserCountryByIp();
            var haveUniversity = false;
            var userData = FormsAuthenticationService.GetUserData();
            if (userData != null && userData.UniversityId.HasValue)
            {
                haveUniversity = true;
            }
           
            ViewBag.country = country;
            ViewBag.haveUniversity = haveUniversity.ToString().ToLower();
           
            return View("_SelectUni");
        }
        [HttpGet, Ajax]
        public ActionResult SearchUniversity(string term)
        {
            try
            {
                var retVal = m_UniversitySearch.Value.SearchUniversity(term);
                return this.CdJson(new JsonResponse(true, retVal));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("SeachUniversity term:  " + term, ex);
                return this.CdJson(new JsonResponse(false));
            }
        }

        [HttpGet, Ajax]
        public async Task<ActionResult> GetFriends(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                return this.CdJson(null);
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

                return this.CdJson(new JsonResponse(true, suggestedUniversity));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Library Get friends authkey=" + authToken, ex);
                return this.CdJson(new JsonResponse(false));
            }
        }

        [NonAction]
        private string GetUserCountryByIp()
        {
            string userIp = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(userIp))
            {
                userIp = Request.ServerVariables["REMOTE_ADDR"];
            }
            if (Request.IsLocal)
            {
                userIp = "81.218.135.73";
            }
            var ipNumber = Ip2Long(userIp);
            //var ipAddress = IPAddress.Parse(userIp);

            //var ipNumber2 = BitConverter.ToInt64(ipAddress.GetAddressBytes().Reverse().ToArray(), 0);

            return ZboxReadService.GetLocationByIp(ipNumber);

        }
        [NonAction]
        private long Ip2Long(string ip)
        {
            string[] ipBytes;
            double num = 0;
            if (!string.IsNullOrEmpty(ip))
            {
                ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }
            }
            return (long)num;
        }

        [HttpGet]
        [Ajax]
        //[AjaxCache(TimeConsts.Minute * 30)]
        public ActionResult Nodes(Guid? section, int page = 0, OrderBy order = OrderBy.Name)
        {
            var userDetail = FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university), JsonRequestBehavior.AllowGet);
            }
            var query = new GetLibraryNodeQuery(userDetail.UniversityId.Value, section, GetUserId(), page, order);
            var result = ZboxReadService.GetLibraryNode(query);
            return this.CdJson(new JsonResponse(true, result));

        }

        #region DeleteNode
        [HttpPost, Ajax]
        public JsonResult DeleteNode(Guid id)
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            if (!userDetail.UniversityId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university));
            }

            var command = new DeleteNodeFromLibraryCommand(id, userDetail.UniversityId.Value);
            ZboxWriteService.DeleteNodeLibrary(command);
            return Json(new JsonResponse(true));

        }
        #endregion

        #region RenameNode
        [HttpPost, Ajax]
        public JsonResult RenameNode(RenameLibraryNode model)
        {
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
                var command = new RenameNodeCommand(model.NewName, model.Id, userDetail.UniversityId.Value);

                ZboxWriteService.RenameNodeLibrary(command);
                return Json(new JsonResponse(true));
            }
            catch (ArgumentException ex)
            {
                return Json(new JsonResponse(false, ex.Message));
            }
        }
        #endregion

        #region Create
        [HttpPost, Ajax]
        public ActionResult Create(CreateLibraryItem model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetModelStateErrors()));
            }
            var userDetail = FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                return this.CdJson(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university));
            }
            if (userDetail.UniversityId.Value != GetUserId())
            {
                return this.CdJson(new JsonResponse(false, "you unauthorized to add departments"));
            }
            try
            {
                var id = m_IdGenerator.Value.GetId();
                var command = new AddNodeToLibraryCommand(model.Name, id, userDetail.UniversityId.Value, model.ParentId);
                ZboxWriteService.AddNodeToLibrary(command);
                var result = new NodeDto { Id = id, Name = model.Name };
                return this.CdJson(new JsonResponse(true, result));
            }
            catch (ArgumentException)
            {
                return this.CdJson(new JsonResponse(false, "unspecified error"));
            }
        }

        [HttpPost]
        [Ajax]
        [ValidateAntiForgeryToken]
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
            try
            {
                var userId = GetUserId();
                var command = new CreateAcademicBoxCommand(userId, model.CourseName,
                                                           model.CourseId, model.Professor, model.ParentId);
                var result = ZboxWriteService.CreateBox(command);
                return this.CdJson(new JsonResponse(true, new { result.NewBox.Url}));
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
        #endregion

       [Ajax]
        [HttpGet]
        public ActionResult NewUniversity()
        {
            return PartialView("_AddSchoolDialog", new CreateUniversity());
        }

        [HttpPost]
        [Ajax]
        public async Task<ActionResult> UniversityRequest(CreateUniversity model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            await m_TableProvider.Value.InsertUserRequestAsync(new Zbox.Infrastructure.Storage.Entities.NewUniversity(model.Name, GetUserId(), model.Country, model.SchoolType));
            return Json(new JsonResponse(true));
        }

        [Ajax, HttpGet]
        public ActionResult InsertCode(long universityId)
        {
            var userData = m_UserProfile.Value.GetUserData(ControllerContext);
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
            ViewBag.userName = userData.Name;
            return this.CdJson(new JsonResponse(true, new { html = RenderRazorViewToString("InsertCode", new Models.Account.Settings.University { UniversityId = universityId }) }));
        }
        [Ajax, HttpGet]
        public ActionResult InsertId(long universityId)
        {
            var userData = m_UserProfile.Value.GetUserData(ControllerContext);
            switch (universityId)
            {
                default:
                    ViewBag.AgudaSentence = "המאגר האקדמי של המכללה למינהל פתוח לכל חברי אגודת הסטודנטים של המכללה למינהל.  אימות חברי אגודה ע\"י מספר ת\"ז";
                    //ViewBag.AgudaName = "המרכז ללימודים אקדמיים אור יהודה";
                    ViewBag.AgudaMail = "aguda4u.co.il@gmail.com";
                    ViewBag.AgudaPhone = "או צרו קשר ישירות עם מזכירות האגודה בטלפון 03-9628930";
                    break;
            }
            ViewBag.userName = userData.Name;
            return this.CdJson(new JsonResponse(true, new { html = RenderRazorViewToString("InsertID", null) }));
        }

        [Ajax, HttpGet]
        public async Task<ActionResult> SelectDepartment(long universityId)
        {
            var retVal = await ZboxReadService.GetDepartmentList(universityId);
            return this.CdJson(new JsonResponse(true, new { html = RenderRazorViewToString("SelectDepartment", retVal) }));
        }

        [Ajax, HttpGet]
        public async Task<ActionResult> Departments()
        {

            var query = new GetUserMinProfileQuery(GetUserId());
            var result = await ZboxReadService.GetUserMinProfile(query);


            if (result.Score < UserController.AdminReputation)
            {
                return this.CdJson(new JsonResponse(false));
            }
            var userDetail = FormsAuthenticationService.GetUserData();
            var universityId = userDetail.UniversityWrapperId ?? userDetail.UniversityId.Value;

            var retVal = await ZboxReadService.GetDepartmentList(universityId);
            return this.CdJson(new JsonResponse(true, retVal));
        }

        [HttpPost, Ajax]
        public ActionResult Verify(string code)
        {
            var isValid = code == "cloudvivt";
            return this.CdJson(new JsonResponse(true, isValid));
        }
    }
}
