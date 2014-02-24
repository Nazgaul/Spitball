using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.DTOs.Library;
using Zbang.Zbox.ViewModel.Queries.Library;
using System.Web.UI;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using System.Collections.Generic;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    public class LibraryController : BaseController
    {
        private readonly Lazy<ITableProvider> m_TableProvider;
        private readonly Lazy<IUserProfile> m_UserProfile;
        private readonly Lazy<IIdGenerator> m_IdGenerator;
        private readonly Lazy<IZboxCacheReadService> m_ZboxCacheReadService;

        public LibraryController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            Lazy<IZboxCacheReadService> zboxCacheReadService,
            Lazy<ITableProvider> tableProvider,
            Lazy<IUserProfile> userProfile,
            IFormsAuthenticationService formsAuthenticationService,
            Lazy<IIdGenerator> idGenerator)
            : base(zboxWriteService, zboxReadService,
            formsAuthenticationService)
        {
            m_TableProvider = tableProvider;
            m_UserProfile = userProfile;
            m_IdGenerator = idGenerator;
            m_ZboxCacheReadService = zboxCacheReadService;
        }


        [UserNavNWelcome]
        [HttpGet]
        [AjaxCache(TimeConsts.Minute * 10)]
        [NoUniversity]
        [CompressFilter]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "none")]
        public async Task<ActionResult> Index(Guid? LibId)
        {
            var userDetail = m_FormsAuthenticationService.GetUserData();
            if (userDetail == null || !userDetail.UniversityId.HasValue)
            {
                return RedirectToAction("Choose");
            }

            var universityWrapper = userDetail.UniversityWrapperId ?? userDetail.UniversityId.Value;

            var query = new GetUniversityDetailQuery(userDetail.UniversityId.Value,
                universityWrapper);

            var result = await m_ZboxCacheReadService.Value.GetUniversityDetail(query);
            if (result.Id == 0)
            {
                return RedirectToAction("Choose");
            }
            //TODO: bring with one roundtrip
            var queryNodes = new GetLibraryNodeQuery(userDetail.UniversityId.Value, LibId, GetUserId(), 0, OrderBy.LastModified);
            var data = m_ZboxReadService.GetLibraryNode(queryNodes);
            data.Boxes.Elem = AssignUrl(data.Boxes.Elem);
            JsonNetSerializer serializer = new JsonNetSerializer();

            ViewBag.data = serializer.Serialize(data);

            if (Request.IsAjaxRequest())
            {
                return PartialView(result);
            }
            return View(result);

        }

        private IEnumerable<BoxDto> AssignUrl(IEnumerable<BoxDto> data)
        {
            UrlBuilder builder = new UrlBuilder(this.HttpContext);
            foreach (var item in data)
            {
                item.Url = builder.BuildBoxUrl(item.BoxType, item.Id, item.Name, item.UniName);
            }
            return data;

        }

        [HttpGet]
        public async Task<ActionResult> Choose()
        {
            var country = GetUserCountryByIP();

            var query = new GetUniversityByPrefixQuery();
            var result = await m_ZboxCacheReadService.Value.GetUniversityListByPrefix(query);

            var haveUniversity = false;
            var userData = m_FormsAuthenticationService.GetUserData();
            if (userData != null && userData.UniversityId.HasValue)
            {
                haveUniversity = true;
            }
            JsonNetSerializer serializer = new JsonNetSerializer();
            ViewBag.data = serializer.Serialize(result.OrderByDescending(o => o.MemberCount));
            //result;
            ViewBag.country = country;
            ViewBag.haveUniversity = haveUniversity.ToString().ToLower();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_SelectUni");
            }
            return View("_SelectUni");
        }

        [NonAction]
        private string GetUserCountryByIP()
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
            var ipNumber = IP2Long(userIp);
            //var ipAddress = IPAddress.Parse(userIp);

            //var ipNumber2 = BitConverter.ToInt64(ipAddress.GetAddressBytes().Reverse().ToArray(), 0);

            return m_ZboxReadService.GetLocationByIP(ipNumber);

        }
        [NonAction]
        private long IP2Long(string ip)
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
        [AjaxCache(TimeConsts.Minute * 30)]
        public ActionResult Nodes(Guid? section, int page = 0, OrderBy order = OrderBy.Name)
        {
            var userDetail = m_FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university), JsonRequestBehavior.AllowGet);
            }
            var query = new GetLibraryNodeQuery(userDetail.UniversityId.Value, section, GetUserId(), page, order);
            var result = m_ZboxReadService.GetLibraryNode(query);
            result.Boxes.Elem = AssignUrl(result.Boxes.Elem);
            return this.CdJson(new JsonResponse(true, result));
            //return Json(new JsonResponse(true, result), JsonRequestBehavior.AllowGet);

        }

        #region DeleteNode
        [HttpPost, Ajax]
        public JsonResult DeleteNode(Guid id)
        {
            var userDetail = m_FormsAuthenticationService.GetUserData();
            if (!userDetail.UniversityId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university));
            }

            var command = new DeleteNodeFromLibraryCommand(id, userDetail.UniversityId.Value);
            m_ZboxWriteService.DeleteNodeLibrary(command);
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
            var userDetail = m_FormsAuthenticationService.GetUserData();
            if (!userDetail.UniversityId.HasValue)
            {
                return Json(new JsonResponse(false, LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university));
            }
            try
            {
                var command = new RenameNodeCommand(model.NewName, model.Id, userDetail.UniversityId.Value);

                m_ZboxWriteService.RenameNodeLibrary(command);
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
            var userDetail = m_FormsAuthenticationService.GetUserData();

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
                //var id = Guid.NewGuid();
                //var color = GenerateNodeColor();
                var command = new AddNodeToLibraryCommand(model.Name, id, userDetail.UniversityId.Value, model.ParentId);
                m_ZboxWriteService.AddNodeToLibrary(command);
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
            var userDetail = m_FormsAuthenticationService.GetUserData();

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
                var result = m_ZboxWriteService.CreateBox(command);

                //TODO: User name can come from cookie detail one well do that
                //TODO: this is not good
                UrlBuilder builder = new UrlBuilder(HttpContext);
                var retVal = new BoxDto(result.NewBox.Id, command.BoxName,
                                        UserRelationshipType.Subscribe, 0, null,
                                        0, 0, command.CourseCode, command.Professor, BoxType.Academic, result.UserName,
                                        builder.BuildBoxUrl(BoxType.Academic, result.NewBox.Id, command.BoxName, result.UserName)
                                        );
                return this.CdJson(new JsonResponse(true, retVal));
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


        [HttpGet]
        [Route("library/search/{query}")]
        public ActionResult Search(string query)
        {
            return RedirectToActionPermanent("Index", "Seach", new { q = query });
        }


        //[HttpGet]
        //[Ajax]
        //[ActionName("University")]
        //[AjaxCache(TimeToCache = TimeConsts.Minute * 5)]
        //[OutputCache(Duration = TimeConsts.Minute * 20, VaryByParam = "term;country;page", Location = OutputCacheLocation.Server)]
        //public async Task<ActionResult> UniversityList(string term, string country, int page = 0)
        //{
        //    if (string.IsNullOrWhiteSpace(country))
        //    {
        //        country = GetUserCountryByIP();
        //    }
        //    var query = new GetUniversityByPrefixQuery(GetUserId(), page, term, country);
        //    var result = await m_ZboxCacheReadService.Value.GetUniversityListByPrefix(query);
        //    //var result = m_ZboxReadService.GetUniversityByPrefix(query);
        //    return this.CdJson(new JsonResponse(true, result));
        //}




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

        [Ajax, HttpGet, AjaxCache(TimeToCache = TimeConsts.Second)]
        public ActionResult InsertCode(long uid)
        {
            var userData = m_UserProfile.Value.GetUserData(ControllerContext);
            switch (uid)
            {
                case 19878:
                    ViewBag.AgudaName = "מכללת אשקלון - היחידה ללימודי חוץ";
                    ViewBag.AgudaMail = "shivok@ash-college.ac.il";

                    ViewBag.AgudaPhone = "או צרו קשר ישירות עם מזכירות האגודה בטלפון\n08-6789250.";
                    break;
                case 1173:
                    ViewBag.AgudaName = "אגודת הסטודנטים בסימינר הקיבוצים";
                    ViewBag.AgudaMail = "academic.aguda@smkb.ac.il";
                    ViewBag.AgudaPhone = string.Empty;
                    break;
                case 22906:
                    ViewBag.AgudaName = "תות תקשורת ותוצאות";
                    ViewBag.AgudaMail = "kipigilad@walla.com‏";

                    ViewBag.AgudaPhone = "או צרו קשר ישירות עם מזכירות האגודה בטלפון\n054-3503509.";
                    break;
                default:
                    ViewBag.AgudaName = "המרכז ללימודים אקדמיים אור יהודה";
                    ViewBag.AgudaMail = "aguda@mla.ac.il";
                    ViewBag.AgudaPhone = "או צרו קשר ישירות עם מזכירות האגודה בטלפון\n072-2220692.";
                    break;
            }
            ViewBag.userName = userData.Name;
            return PartialView(new Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings.University() { UniversityId = uid });
        }

        [Ajax, HttpGet, AjaxCache(TimeToCache = TimeConsts.Second)]
        public async Task<ActionResult> SelectDepartment(long universityId)
        {
            var retVal = await m_ZboxReadService.GetDepartmentList(universityId);
            return this.CdJson(new JsonResponse(true, new { html = RenderRazorViewToString("SelectDepartment", retVal) }));
        }

        [Ajax, HttpGet, AjaxCache(TimeToCache = TimeConsts.Second)]
        public async Task<ActionResult> Departments()
        {
            var userDetail = m_FormsAuthenticationService.GetUserData();
            if (userDetail.Score < UserController.AdminReputation)
            {
                return this.CdJson(new JsonResponse(false));
            }
            var universityId = userDetail.UniversityWrapperId ?? userDetail.UniversityId.Value;

            var retVal = await m_ZboxReadService.GetDepartmentList(universityId);
            return this.CdJson(new JsonResponse(true, retVal));
        }
    }
}
