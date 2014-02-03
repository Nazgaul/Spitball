using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Mvc3WebRole.Factories;
using Zbang.Zbox.Mvc3WebRole.Helpers;
using Zbang.Zbox.Mvc3WebRole.Models;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.DTOs.Library;
using Zbang.Zbox.ViewModel.Queries.Library;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    public class LibraryController : BaseController
    {
        public LibraryController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        { }


        [ZboxAuthorize]
        [OutputCache(Duration = TimeConsts.Day, Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult Index(Guid? section)
        {
            return View("~/Views/Home/Index.cshtml");
        }

        [ChildActionOnly]
        public ActionResult MainView()
        {
            var userDetail = m_FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                return PartialView("_MainView", new UniversityViewModel { Exists = false });
            }

            var query = new GetUniversityDetailQuery(userDetail.UniversityId.Value);
            var result = m_ZboxReadService.GetUniversityDetail(query);
            UniversityViewModel model = new UniversityViewModel
            {
                Name = result.Name,
                Image = result.Image,
                Exists = true
            };

            return PartialView("_MainView", model);
        }

        [ZboxAuthorize]
        [Ajax]
        public ActionResult Nodes(Guid? section)
        {
            //var userDetail = m_FormsAuthenticationService.GetUserData();

            //if (!userDetail.UniversityId.HasValue)
            //{
            //    return Json(new JsonResponse(false, "You need to sign up for university"), JsonRequestBehavior.AllowGet);
            //}
            //var query = new GetLibraryNodeQuery(userDetail.UniversityId.Value, section, GetUserId());
            //var result = m_ZboxReadService.GetLibraryChildren(query);

            return Json(new JsonResponse(true, string.Empty), JsonRequestBehavior.AllowGet);
        }
        [ZboxAuthorize]
        [Ajax]
        public ActionResult ParentNode(Guid section)
        {
            //var userDetail = m_FormsAuthenticationService.GetUserData();

            //if (!userDetail.UniversityId.HasValue)
            //{
            //    return Json(new JsonResponse(false, "You need to sign up for university"), JsonRequestBehavior.AllowGet);
            //}
            //var query = new GetLibraryNodeQuery(userDetail.UniversityId.Value, section, GetUserId());
            //var result = m_ZboxReadService.GetParentDetails(query);

            return Json(new JsonResponse(true, string.Empty), JsonRequestBehavior.AllowGet);
        }

        #region Create
        [HttpPost, Ajax, ZboxAuthorize]
        public JsonResult Create(CreateLibraryItem model)
        {


            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userDetail = m_FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "You need to sign up for university");
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userId = GetUserId();
            Guid id = Guid.NewGuid();
            var color = GenerateNodeColor();
            var command = new AddNodeToLibraryCommand(model.Name, id, userDetail.UniversityId.Value, model.ParentId, color);
            m_ZboxWriteService.AddNodeToLibrary(command);
            var result = new NodeDto { Color = color, Id = id, Name = model.Name };
            return Json(new JsonResponse(true, result));
        }

        [NonAction]
        private string GenerateNodeColor()
        {
            string[] ColorSchema = { "#62328F", "#464646", "#146EB5", "#111", "#17A099", "#83B641", "#E5A13D", "#DC552A", "#952262", "#D91C7A" };
            Random rand = new Random();
            var position = rand.Next(ColorSchema.Length - 1);
            return ColorSchema[position];
        }

        [HttpPost]
        [ZboxAuthorize]
        [Ajax]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBox(CreateAcademicBox model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {
                //var userId = GetUserId();
                //var command = new CreateAcademicBoxCommand(userId, model.BoxName, model.Description,
                //    model.CourseId, model.Professor, model.ParentId);
                //var result = m_ZboxWriteService.CreateBox(command);
                //var shortBoxid = m_ShortToLongCode.LongToShortCode(result.NewBox.Id);
                ////TODO: User name can come from cookie detail one well do that
                //var retVal = new BoxDto(shortBoxid, command.BoxName,
                //     DateTime.UtcNow,0,string.E UserRelationshipType.Subscribe, 0, null,
                //     0, 0);
                return Json(new JsonResponse(true, string.Empty));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("CreateAcademic user: {0} model: {1}", GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, "Problem with Create new box");
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }
        #endregion
        [HttpGet]
        [ZboxAuthorize]
        public ActionResult Search(string query)
        {
            if (Request.IsAjaxRequest())
            {
            }
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new JsonResponse(false, "need to have query"), JsonRequestBehavior.AllowGet);
            }
            var userDetail = m_FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                return Json(new JsonResponse(false, "You need to sign up for university"));
            }

            var searchFactory = new SearchQueryFactory();

            var searchQuery = searchFactory.GetQuery(query, 0, SearchType.Library, userDetail.UniversityId.Value);
            var result = m_ZboxReadService.Search(searchQuery);
            return Json(new JsonResponse(true, result), JsonRequestBehavior.AllowGet);
        }
    }
}
