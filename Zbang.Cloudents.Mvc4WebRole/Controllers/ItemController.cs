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
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [NoUniversity]
    public class ItemController : BaseController
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly ICloudBlockProvider m_CloudBlobProvider;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IQueueProvider m_QueueProvider;
        private readonly Lazy<IGuidIdGenerator> m_GuidGenerator;


        public ItemController(
            IBlobProvider blobProvider,
            IFileProcessorFactory fileProcessorFactory,
            IQueueProvider queueProvider, Lazy<IGuidIdGenerator> guidGenerator, ICloudBlockProvider cloudBlobProvider)
        {
            m_BlobProvider = blobProvider;
            m_FileProcessorFactory = fileProcessorFactory;
            m_QueueProvider = queueProvider;
            m_GuidGenerator = guidGenerator;
            m_CloudBlobProvider = cloudBlobProvider;
        }


        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult IndexPartial()
        {
            return PartialView("Index2");
        }

        [BoxPermission("boxId", Order = 2)]
        [RedirectToMobile(Order = 1)]
        //[DonutOutputCache(VaryByCustom = CustomCacheKeys.Lang,
        //  Duration = TimeConsts.Hour * 1, VaryByParam = "itemid",
        //  Location = OutputCacheLocation.Server, Order = 4)]
        //[RedirectToMobile(Order = 1)]
        public async Task<ActionResult> Index(long boxId, long itemid, string itemName, string universityName, string boxName)
        {

            try
            {

                var query = new GetFileSeoQuery(itemid);
                var model = await ZboxReadService.GetItemSeoAsync(query);
                if (model == null)
                {
                    throw new ItemNotFoundException();
                }
                if (Request.Url != null && model.Url != Server.UrlDecode(Request.Url.AbsolutePath))
                {
                    throw new ItemNotFoundException();
                }
                if (model.Discriminator.ToUpper() != "FILE") return View("Empty");
                ViewBag.imageSrc = ViewBag.fbImage = "https://az779114.vo.msecnd.net/preview/" + model.ImageUrl +
                                  ".jpg?width=1200&height=630&mode=crop";
                if (string.IsNullOrEmpty(model.Country)) return View("Empty");

                var culture = Languages.GetCultureBaseOnCountry(model.Country);
                BaseControllerResources.Culture = culture;
                var seoItemName = Path.GetFileNameWithoutExtension(model.Name);

                ViewBag.title = string.Format("{0} | {1} | {2} | {3} | {4}",
                    model.BoxName, seoItemName, model.DepartmentName, model.UniversityName, BaseControllerResources.Cloudents);

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


        public async Task<ActionResult> ShortUrl(string item62Id)
        {
            var base62 = new Base62(item62Id);
            var query = new GetFileSeoQuery(base62.Value);
            var model = await ZboxReadService.GetItemSeoAsync(query);
            return RedirectPermanent(model.Url);
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
                var tItem = ZboxReadService.GetItem2Async(query);

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
                //retVal.Name = Path.GetFileNameWithoutExtension(retVal.Name);
                retVal.ShortUrl = UrlConsts.BuildShortItemUrl(new Base62(itemId).ToString());
                return JsonOk(new
                {
                    retVal.Blob,
                    retVal.BoxUrl,
                    //retVal.Comments,
                    retVal.Name,
                    //retVal.Navigation.Next,
                    //retVal.Navigation.Previous,
                    //retVal.NumberOfDownloads,
                    //retVal.NumberOfViews,
                    //retVal.Owner,
                    //retVal.OwnerId,
                    //retVal.PrintUrl,
                    //retVal.Rate,
                    retVal.ShortUrl,
                    //retVal.UpdateTime,
                    //retVal.UserType
                });
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
        [BoxPermission("boxId", Order = 1)]
        [RemoveBoxCookie(Order = 2)]
        public async Task<ActionResult> Download(long boxId, long itemId)
        {
            const string defaultMimeType = "application/octet-stream";
            var userId = User.GetUserId();

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

            var blob = m_CloudBlobProvider.GetFile(filedto.Blob);
            var contentType = defaultMimeType;

            if (!string.IsNullOrWhiteSpace(blob.Properties.ContentType))
            {
                contentType = blob.Properties.ContentType;
            }
            return new BlobFileStream(blob, contentType, item.Name, true);
        }

        //[HttpGet, ZboxAuthorize]
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

        /// <summary>
        /// Used to rename file name - item name cannot be changed
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ZboxAuthorize, HttpPost]
        public JsonResult Rename(Rename model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }

            var userId = User.GetUserId();
            try
            {
                var command = new ChangeFileNameCommand(model.Id, model.NewName, userId);
                var result = ZboxWriteService.ChangeFileName(command);
                return JsonOk(new
                {
                    name = result.Name,
                    url = result.Url
                    //queryString = UrlBuilder.NameToQueryString(result.Name)
                });
            }

            catch (UnauthorizedAccessException)
            {
                return JsonError("You need to follow this box in order to change file name");
            }
            catch (ArgumentException ex)
            {
                return JsonError(ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("ChangeFileName newFileName {0} ItemUid {1} userId {2}", model.NewName, model.Id, userId), ex);
                return JsonError("Error");
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
        public async Task<JsonResult> Delete(long itemId, long boxId)
        {
            try
            {
                var command = new DeleteItemCommand(itemId, User.GetUserId(), boxId);
                await ZboxWriteService.DeleteItemAsync(command);
                return JsonOk(itemId);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteItem user: {0} boxid: {1} itemId {2}", User.GetUserId(), boxId, itemId), ex);
                return JsonError();
            }
        }



        [ZboxAuthorize, HttpPost, RemoveBoxCookie]
        public async Task<JsonResult> Like(RateModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var id = m_GuidGenerator.Value.GetId();
                var command = new RateItemCommand(model.ItemId, User.GetUserId(), id, model.BoxId);
                await ZboxWriteService.RateItemAsync(command);

                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rate user: {0} itemId {1}", User.GetUserId(), model.ItemId), ex);
                return JsonError();
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

        //[HttpGet]
        //public ActionResult FullScreen()
        //{
        //    try
        //    {
        //        return PartialView();
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("FullScreen ", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}

        //[HttpPost, ZboxAuthorize,]
        //public async Task<JsonResult> AddComment(NewAnnotation model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
        //    }
        //    try
        //    {
        //        var command = new AddAnnotationCommand(model.Comment, model.ItemId, User.GetUserId(), model.BoxId);
        //        await ZboxWriteService.AddAnnotationAsync(command);
        //        return JsonOk(command.AnnotationId);
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        return JsonError();
        //    }
        //}
        //[HttpPost]
        //[ZboxAuthorize]
        //public JsonResult DeleteComment(DeleteItemComment model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
        //    }
        //    var command = new DeleteItemCommentCommand(model.CommentId, User.GetUserId());
        //    ZboxWriteService.DeleteAnnotation(command);
        //    return Json(new JsonResponse(true));
        //}
        //[HttpPost]
        //[ZboxAuthorize]
        //public async Task<JsonResult> ReplyComment(ReplyItemComment model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return JsonError(new { error = GetModelStateErrors() });

        //    }
        //    var command = new AddReplyToAnnotationCommand(User.GetUserId(), model.ItemId, model.Comment, model.CommentId, model.BoxId);
        //    await ZboxWriteService.AddReplyAnnotationAsync(command);
        //    return JsonOk(command.ReplyId);
        //}

        //[HttpPost, ZboxAuthorize]
        //public JsonResult DeleteCommentReply(DeleteItemCommentReply model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
        //    }
        //    var command = new DeleteItemCommentReplyCommand(User.GetUserId(), model.ReplyId);
        //    ZboxWriteService.DeleteItemCommentReply(command);
        //    return Json(new JsonResponse(true));
        //}

        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]
        public ActionResult ItemRegisterPartial()
        {
            try
            {
                return PartialView("_ItemRegister");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_ItemRegister", ex);
                return Json(new JsonResponse(false));
            }
        }
    }
}
