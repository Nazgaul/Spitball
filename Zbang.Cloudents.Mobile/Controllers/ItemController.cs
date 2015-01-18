using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using Zbang.Cloudents.Mobile.Controllers.Resources;
using Zbang.Cloudents.Mobile.Extensions;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Cloudents.Mobile.Models;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [NoUniversity]
    public class ItemController : BaseController
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IQueueProvider m_QueueProvider;


        public ItemController(
            IBlobProvider blobProvider,
            IFileProcessorFactory fileProcessorFactory,
            IQueueProvider queueProvider
            )
        {
            m_BlobProvider = blobProvider;
            m_FileProcessorFactory = fileProcessorFactory;
            m_QueueProvider = queueProvider;
        }





        [NoCache]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [Route("Item/{universityName}/{boxId:long}/{boxName}/{itemid:long:min(0)}/{itemName}", Name = "Item")]
        public ActionResult Index(long boxId, long itemid, string itemName, string universityName, string boxName)
        {
            try
            {
                var userId = User.GetUserId(false); // not really needs it

                var query = new GetItemQuery(userId, itemid, boxId);
                var item = ZboxReadService.GetItem(query);
                if (item.BoxId != boxId)
                {
                    throw new ItemNotFoundException();
                }
                if (itemName != UrlBuilder.NameToQueryString(item.Name))
                {
                    throw new ItemNotFoundException();
                }

                if (!string.IsNullOrEmpty(item.Country))
                {
                    var culture = Languages.GetCultureBaseOnCountry(item.Country);
                    BaseControllerResources.Culture = culture;
                    var seoItemName = item.Name;
                    var file = item as FileWithDetailDto;
                    if (file != null)
                    {
                        seoItemName = file.NameWOExtension;
                    }
                    ViewBag.title = string.Format("{0} {1} | {2} | {3}", BaseControllerResources.TitlePrefix, item.BoxName, seoItemName, BaseControllerResources.Cloudents);
                }
                if (!string.IsNullOrEmpty(item.Description))
                {
                    var metaDescription = item.Description.RemoveEndOfString(197);
                    ViewBag.metaDescription = metaDescription.Length == 197 ? metaDescription + "..." : metaDescription;
                }
                ViewBag.Description = item.Description;
                if (Request.IsAjaxRequest())
                {
                    return PartialView();
                }
                return View();
            }
            catch (BoxAccessDeniedException)
            {
                return RedirectToAction("MembersOnly", "Error", new { returnUrl = Request.Url.AbsolutePath });
            }
            catch (ItemNotFoundException)
            {
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On item load boxid = " + boxId + " ,itemid = " + itemid, ex);
                return RedirectToAction("Index", "Error");
            }

        }

        /// <summary>
        /// Ajax Request - item data
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpGet]
        [BoxPermission("boxId")]
        public async Task<ActionResult> Load(long boxId, long itemId)
        {
            try
            {
                var userId = User.GetUserId(false);

                var query = new GetItemQuery(userId, itemId, boxId);
                var tItem = ZboxReadService.GetItem2(query);

                var tTransAction = m_QueueProvider.InsertMessageToTranactionAsync(
                      new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = itemId,
                            Action = (int)StatisticsAction.View
                        }
                    }, userId, DateTime.UtcNow));

                await Task.WhenAll(tItem, tTransAction);
                var retVal = tItem.Result;
                retVal.UserType = ViewBag.UserType;
                return Json(new JsonResponse(true, retVal));
            }
            catch (BoxAccessDeniedException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            catch (ItemNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On item load boxid = " + boxId + " ,itemid = " + itemId, ex);
                return Json(new JsonResponse(false));
            }

        }

        /// <summary>
        /// Download Item
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="itemId"></param>
        /// <returns>if link redirect to link if file download</returns>
        [ZboxAuthorize]
        [Route("Item/{universityName}/{boxId:long}/{boxName}/{itemid:long:min(0)}/{itemName}/download", Name = "ItemDownload")]
        [Route("D/{boxId:long:min(0)}/{itemId:long:min(0)}", Name = "ItemDownload2")]
        [NoEtag]
        [BoxPermission("boxId")]
        public async Task<ActionResult> Download(long boxId, long itemId)
        {
            const string defaultMimeType = "application/octet-stream";
            var userId = User.GetUserId(false);

            var query = new GetItemQuery(userId, itemId, boxId);

            var item = ZboxReadService.GetItem(query);


            var filedto = item as FileWithDetailDto;
            if (filedto == null) // link
            {
                return Redirect(item.Blob);
            }
            await m_QueueProvider.InsertMessageToTranactionAsync(
                   new StatisticsData4(new List<StatisticsData4.StatisticItemData>
                    {
                        new StatisticsData4.StatisticItemData
                        {
                            Id = itemId,
                            Action = (int)StatisticsAction.Download
                        }
                    }, userId, DateTime.UtcNow));

            var blob = m_BlobProvider.GetFile(filedto.Blob);
            var contentType = defaultMimeType;
            if (!string.IsNullOrWhiteSpace(blob.Properties.ContentType))
            {
                contentType = blob.Properties.ContentType;
            }
            return new BlobFileStream(blob, contentType, item.Name, true);
        }


        #region Preview
        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("boxId")]
        [AsyncTimeout(TimeConsts.Minute * 3 * 1000)]
        [JsonHandleError(HttpStatus = HttpStatusCode.BadRequest, ExceptionType = typeof(ArgumentException))]
        public async Task<JsonResult> Preview(string blobName, int index, long id,
            long boxId, CancellationToken cancellationToken, int width = 0, int height = 0)
        {
            Uri uri;
            if (!Uri.TryCreate(blobName, UriKind.Absolute, out uri))
            {
                uri = new Uri(m_BlobProvider.GetBlobUrl(blobName));
            }
            var processor = m_FileProcessorFactory.GetProcessor(uri);
            if (processor == null)
                return
                    JsonOk(
                        new
                        {
                            preview =
                                RenderRazorViewToString("_PreviewFailed",
                                    Url.RouteUrl("ItemDownload2", new { boxId, itemId = id }))
                        });

            try
            {
                var retVal = await processor.ConvertFileToWebSitePreview(uri, width, height, index * 3, cancellationToken);
                if (retVal.Content == null)
                {
                    return JsonOk(new
                    {
                        preview = RenderRazorViewToString("_PreviewFailed",
                            Url.RouteUrl("ItemDownload2", new { boxId, itemId = id }))
                    });

                }
                if (string.IsNullOrEmpty(retVal.ViewName))
                {
                    return JsonOk(new { preview = retVal.Content.First() });
                }

                return JsonOk(new
                {
                    preview = RenderRazorViewToString("_Preview" + retVal.ViewName,
                     new ItemPreview(
                           retVal.Content.Take(3),
                           index,
                           User.Identity.IsAuthenticated))
                });

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("GeneratePreview filename: {0}", blobName), ex);
                if (index == 0)
                {
                    return JsonOk(new
                    {
                        preview = RenderRazorViewToString("_PreviewFailed",
                            Url.RouteUrl("ItemDownload2", new { boxId, itemId = id }))
                    });
                }
                return JsonOk();
            }
        }
        #endregion








        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]
        //public ActionResult ItemRegisterPartial()
        //{
        //    try
        //    {
        //        return PartialView("_ItemRegister");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_ItemRegister", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}
    }
}
