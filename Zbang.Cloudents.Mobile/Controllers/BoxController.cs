using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxHandleError(ExceptionType = typeof(UnauthorizedAccessException), View = "Error")]
    [ZboxHandleError(ExceptionType = typeof(BoxAccessDeniedException), View = "Error")]
    [NoUniversity]
    public class BoxController : BaseController
    {


        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage",
            Options = OutputCacheOptions.IgnoreQueryString
            )]
        public PartialViewResult IndexPartial()
        {
            return PartialView("Index");
        }


        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<ActionResult> Data(long id)
        {
            try
            {
                var query = new GetBoxQuery(id);
                var result = await ZboxReadService.GetBox2(query);
                result.UserType = ViewBag.UserType;
                return JsonOk(new
                {
                    result.Name,
                    result.BoxType,
                    result.UserType,
                    result.ProfessorName,
                    result.CourseId
                });
            }
            catch (BoxAccessDeniedException)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            catch (BoxDoesntExistException)
            {
                return HttpNotFound();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Index id {0}", id), ex);
                return JsonError();
            }
        }

        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> Tabs(long id)
        {
            try
            {
                var query = new GetBoxQuery(id);
                var result = await ZboxReadService.GetBoxTabs(query);
                return JsonOk(result);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Tabs id {0} ", id), ex);
                return JsonError();
            }
        }


        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> Items(long id, Guid? tabId, int page)
        {
            try
            {
                var query = new GetBoxItemsPagedQuery(id, tabId, page, 20);
                var result = await ZboxReadService.GetBoxItemsPagedAsync(query) ?? new List<Zbox.ViewModel.Dto.ItemDtos.ItemDto>();
                return JsonOk(result.Select(s => new { s.Name, s.Thumbnail, s.Owner, s.Url }));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxId {0} page {1}", id, page), ex);
                return JsonError();
            }
        }

        /// <summary>
        /// Used for old item view
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tabId"></param>
        /// <returns></returns>
        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> Items2(long id)
        {
            try
            {
                var query = new GetBoxItemsPagedQuery(id, null);
                var result = await ZboxReadService.GetBoxItemsPagedAsync(query) ?? new List<Zbox.ViewModel.Dto.ItemDtos.ItemDto>();
                return JsonOk(result.Select(s => new { s.Id }));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxId {0}", id), ex);
                return JsonError();
            }
        }


    }
}
