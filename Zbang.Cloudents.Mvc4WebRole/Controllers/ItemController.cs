using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [NoUniversity]
    public class ItemController : BaseController
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IQueueProvider m_QueueProvider;
        private readonly Lazy<IIdGenerator> m_IdGenerator;


        public ItemController(
            IBlobProvider blobProvider,
            IFileProcessorFactory fileProcessorFactory,
            IQueueProvider queueProvider,
            Lazy<IIdGenerator> idGenerator
            )
        {
            m_BlobProvider = blobProvider;
            m_FileProcessorFactory = fileProcessorFactory;
            m_QueueProvider = queueProvider;
            m_IdGenerator = idGenerator;
        }


        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(Duration = TimeConsts.Minute * 5,
            Location = OutputCacheLocation.ServerAndClient,
            VaryByCustom = CustomCacheKeys.Lang, Options = OutputCacheOptions.IgnoreQueryString, VaryByParam = "none")]
        public PartialViewResult IndexPartial()
        {
            return PartialView("Index");
        }

        [NoCache]
        [BoxPermission("boxId")]
        public async Task<ActionResult> IndexDesktop(long boxId, long itemid, string itemName, string universityName, string boxName)
        {

            try
            {

                var query = new GetFileSeoQuery(itemid);
                var model = await ZboxReadService.GetItemSeo(query);
                if (model == null)
                {
                    throw new ItemNotFoundException();
                }
                if (Request.Url != null && model.Url != Server.UrlDecode(Request.Url.AbsolutePath))
                {
                    throw new ItemNotFoundException();
                }
                if (model.Discriminator.ToUpper() != "FILE") return View("Empty");
                if (string.IsNullOrEmpty(model.Country)) return View("Empty");

                var culture = Languages.GetCultureBaseOnCountry(model.Country);
                BaseControllerResources.Culture = culture;
                var seoItemName = Path.GetFileNameWithoutExtension(model.Name);

                ViewBag.title = string.Format("{0} {1} | {2} | {3}", BaseControllerResources.TitlePrefix,
                    model.BoxName, seoItemName, BaseControllerResources.Cloudents);

                ViewBag.Description = model.Description;
                if (!string.IsNullOrEmpty(model.Description))
                {
                    var metaDescription = model.Description.RemoveEndOfString(197);
                    ViewBag.metaDescription = metaDescription.Length == 197
                        ? metaDescription + "..."
                        : metaDescription;
                }
                return View("Empty");
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


        [NoCache]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        //[Route("Item/{universityName}/{boxId:long}/{boxName}/{itemid:long:min(0)}/{itemName}", Name = "Item")]
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

        [HttpGet, ZboxAuthorize]
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

        /// <summary>
        /// Used to rename file name - item name cannot be changed
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ZboxAuthorize]
        [HttpPost]
        public JsonResult Rename(Rename model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
            }

            var userId = User.GetUserId();
            try
            {
                var command = new ChangeFileNameCommand(model.Id, model.NewName, userId);
                var result = ZboxWriteService.ChangeFileName(command);
                return Json(new JsonResponse(true, new
                {
                    name = result.Name,
                    url = result.Url
                    //queryString = UrlBuilder.NameToQueryString(result.Name)
                }));
            }

            catch (UnauthorizedAccessException)
            {
                return Json(new JsonResponse(false, "You need to follow this box in order to change file name"));
            }
            catch (ArgumentException ex)
            {
                return Json(new JsonResponse(false, ex.Message));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("ChangeFileName newFileName {0} ItemUid {1} userId {2}", model.NewName, model.Id, userId), ex);
                return Json(new JsonResponse(false, "Error"));
            }


        }

        /// <summary>
        /// Print selected file
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="itemId"></param>
        /// <returns>View with no layout and print command in javascript</returns>
        [ZboxAuthorize]
        [Route("Item/{universityName}/{boxId:long}/{boxName}/{itemId:long:min(0)}/{itemName}/print", Name = "ItemPrint")]
        [BoxPermission("boxId")]
        public async Task<ActionResult> Print(long boxId, long itemId)
        {

            var query = new GetItemQuery(User.GetUserId(false), itemId, boxId);

            var item = ZboxReadService.GetItem(query);



            var filedto = item as FileWithDetailDto;
            if (filedto != null)
            {
                var uri = new Uri(m_BlobProvider.GetBlobUrl(filedto.Blob));



                IEnumerable<string> retVal = null;


                var processor = m_FileProcessorFactory.GetProcessor(uri);
                if (processor != null)
                {
                    var result = await processor.ConvertFileToWebSitePreview(uri, int.MaxValue, int.MaxValue, 0);
                    retVal = result.Content;

                }
                return View(retVal);
            }
            return Redirect(item.Blob);

        }


        [ZboxAuthorize]
        [HttpPost]
        public ActionResult Delete(long itemId, long boxId)
        {
            try
            {
                var command = new DeleteItemCommand(itemId, User.GetUserId(), boxId);
                ZboxWriteService.DeleteItem(command);
                return Json(new JsonResponse(true, itemId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteItem user: {0} boxid: {1} itemId {2}", User.GetUserId(), boxId, itemId), ex);
                return Json(new JsonResponse(false));
            }
        }



        [ZboxAuthorize]
        [HttpPost]
        public JsonResult Rate(RateModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
            }
            try
            {
                var id = m_IdGenerator.Value.GetId();
                var command = new RateItemCommand(model.ItemId, User.GetUserId(), model.Rate, id);
                ZboxWriteService.RateItem(command);

                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rate user: {0} itemId {1}", User.GetUserId(), model.ItemId), ex);
                return Json(new JsonResponse(false));
            }
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


        #region flagItem
        [HttpGet]
        [ZboxAuthorize]
        public PartialViewResult Flag()
        {
            return PartialView("_FlagItem", new FlagBadItem());
        }

        [HttpPost]
        [ZboxAuthorize]
        public JsonResult FlagRequest(FlagBadItem model)
        {

            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }

            m_QueueProvider.InsertMessageToTranaction(new BadItemData(model.BadItem.GetEnumDescription(), model.Other, User.GetUserId(), model.ItemId));
            return Json(new JsonResponse(true));
        }

        #endregion

        [HttpGet]
        public ActionResult FullScreen()
        {
            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("FullScreen ", ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpPost, ZboxAuthorize,]
        public JsonResult AddComment(NewAnnotation model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
            }
            try
            {
                var command = new AddAnnotationCommand(model.Comment, model.ItemId, User.GetUserId());
                ZboxWriteService.AddAnnotation(command);
                return Json(new JsonResponse(true, command.AnnotationId));
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new JsonResponse(false));
            }
        }
        [HttpPost]
        [ZboxAuthorize]
        public JsonResult DeleteComment(DeleteItemComment model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
            }
            var command = new DeleteItemCommentCommand(model.CommentId, User.GetUserId());
            ZboxWriteService.DeleteAnnotation(command);
            return Json(new JsonResponse(true));
        }
        [HttpPost]
        [ZboxAuthorize]
        public JsonResult ReplyComment(ReplyItemComment model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));

            }
            var command = new AddReplyToAnnotationCommand(User.GetUserId(), model.ItemId, model.Comment, model.CommentId);
            ZboxWriteService.AddReplyAnnotation(command);
            return Json(new JsonResponse(true, command.ReplyId));
        }

        [HttpPost, ZboxAuthorize]
        public JsonResult DeleteCommentReply(DeleteItemCommentReply model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
            }
            var command = new DeleteItemCommentReplyCommand(User.GetUserId(), model.ReplyId);
            ZboxWriteService.DeleteItemCommentReply(command);
            return Json(new JsonResponse(true));
        }
    }
}
