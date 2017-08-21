﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
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
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [NoUniversity]
    public class ItemController : BaseController
    {
        private readonly Lazy<IFileProcessorFactory> m_FileProcessorFactory;
        private readonly IQueueProvider m_QueueProvider;
        private readonly Lazy<IGuidIdGenerator> m_GuidGenerator;
        private readonly Lazy<IItemReadSearchProvider> m_ItemSearchProvider;
        private readonly Lazy<IBlobProvider2<FilesContainerName>> m_BlobProviderFiles;

        public ItemController(
            Lazy<IFileProcessorFactory> fileProcessorFactory,
            IQueueProvider queueProvider, Lazy<IGuidIdGenerator> guidGenerator,
            Lazy<IItemReadSearchProvider> itemSearchProvider,
            Lazy<IBlobProvider2<FilesContainerName>> blobProviderFiles)
        {
            m_FileProcessorFactory = fileProcessorFactory;
            m_QueueProvider = queueProvider;
            m_GuidGenerator = guidGenerator;
            m_ItemSearchProvider = itemSearchProvider;
            m_BlobProviderFiles = blobProviderFiles;
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult IndexPartial()
        {
            return PartialView("Index2");
        }

        [BoxPermission("boxId", Order = 1), ActionName("Index")]
        [DonutOutputCache(CacheProfile = "ItemPage", Order = 2)]
        [Route("item/{universityName}/{boxId:long}/{boxName}/{itemId:long}/{itemName}", Name = "Item")]
        public async Task<ActionResult> IndexAsync(long boxId, long itemId, string itemName)
        {
            try
            {
                var query = new GetFileSeoQuery(itemId);
                var model = await ZboxReadService.GetItemSeoAsync(query).ConfigureAwait(false);
                if (model == null)
                {
                    throw new ItemNotFoundException();
                }
                if (UrlConst.NameToQueryString(model.Name) != itemName)
                {
                    throw new ItemNotFoundException();
                }

                if (model.Discriminator.ToUpper() != "FILE") return View("Empty");
                ViewBag.imageSrc = ViewBag.fbImage = "https://az779114.vo.msecnd.net/preview/" + model.ImageUrl +
                                  ".jpg?width=1200&height=630&mode=crop";
                if (string.IsNullOrEmpty(model.Country)) return View("Empty");

                var culture = Languages.GetCultureBaseOnCountry(model.Country);
                SeoBaseUniversityResources.Culture = culture;

                ViewBag.title =
                    $"{model.BoxName} - {model.DepartmentName} - {model.Name} | {SeoBaseUniversityResources.Cloudents}";

                ViewBag.metaDescription = SeoBaseUniversityResources.ItemMetaDescription;
                if (!string.IsNullOrEmpty(model.Description))
                {
                    ViewBag.metaDescription += ":" + model.Description.RemoveEndOfString(100);
                }
                ViewBag.metaDescription = Server.HtmlDecode(ViewBag.metaDescription);
                return View("Empty");
            }
            catch (ItemNotFoundException)
            {
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On item load boxId = " + boxId + " ,itemId = " + itemId, ex);
                return RedirectToAction("Index", "Error");
            }
        }

        [Route(UrlConst.ShortItem, Name = "shortItem"), NoUrlLowercase]
        public async Task<ActionResult> ShortUrlAsync(string item62Id)
        {
            var base62 = new Base62(item62Id);
            var query = new GetFileSeoQuery(base62.Value);
            var model = await ZboxReadService.GetItemSeoAsync(query).ConfigureAwait(false);
            if (model == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return RedirectToRoutePermanent("Item", new
            {
                UniversityName = UrlBuilder.NameToQueryString(model.UniversityName),
                model.BoxId,
                BoxName = UrlBuilder.NameToQueryString(model.BoxName),
                itemid = base62.Value,
                itemName = UrlBuilder.NameToQueryString(model.Name)
            });
        }

        /// <summary>
        /// Ajax Request - item data
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="itemId"></param>
        /// <param name="firstTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpGet, ActionName("Load")]
        [BoxPermission("boxId")]
        public async Task<ActionResult> LoadAsync(long boxId, long itemId, bool? firstTime, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.GetUserId(false);

                var query = new GetItemQuery(userId, itemId, boxId);
                var tItem = ZboxReadService.GetItem2Async(query);

                var tTransAction = m_QueueProvider.InsertMessageToTransactionAsync(
                      new StatisticsData4(
                        new StatisticsData4.StatisticItemData
                        {
                            Id = itemId,
                            Action = (int)StatisticsAction.View
                        }
                    , userId), cancellationToken);

                var tContent = Zbox.Infrastructure.Extensions.TaskExtensions.CompletedTaskString;
                if (firstTime.HasValue && firstTime.Value)
                {
                    var token = CreateCancellationToken(cancellationToken);
                    tContent = m_ItemSearchProvider.Value.ItemContentAsync(itemId, token.Token);
                }
                await Task.WhenAll(tItem, tTransAction, tContent).ConfigureAwait(false);
                var retVal = tItem.Result;

                return JsonOk(new
                {
                    retVal.Blob,
                    retVal.Name,
                    retVal.Navigation.Next,
                    retVal.Navigation.Previous,
                    retVal.Owner,
                    retVal.OwnerId,
                    retVal.OwnerBadges,
                    retVal.OwnerScore,
                    retVal.Type,
                    retVal.Like,
                    retVal.Likes,
                    retVal.Date,
                    fileContent = tContent.Result
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
                TraceLog.WriteError("On item load boxId = " + boxId + " ,itemId = " + itemId, ex);
                return JsonError();
            }
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpGet, ActionName("Comment")]
        [BoxPermission("boxId")]
        public async Task<ActionResult> CommentsAsync(long itemId)
        {
            var result = await ZboxReadService.GetItemCommentsAsync(new ItemCommentQuery(itemId)).ConfigureAwait(false);
            return JsonOk(result);
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
        [NoEtag, ActionName("Download")]
        [BoxPermission("boxId")]
        [RemoveBoxCookie]
        public async Task<ActionResult> DownloadAsync(long boxId, long itemId)
        {
            var userId = User.GetUserId();

            var t1 = m_QueueProvider.InsertMessageToTransactionAsync(
                   new StatisticsData4(
                        new StatisticsData4.StatisticItemData
                        {
                            Id = itemId,
                            Action = (int)StatisticsAction.Download
                        }
                    , userId));
            var t2 = ZboxReadService.GetItemDetailApiAsync(itemId);

            var userType = ViewBag.UserType as UserRelationshipType?;
            var t3 = Task.CompletedTask;
            if (!userType.HasValue || (int)userType.Value < 2)
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(userId, boxId);
                t3 = ZboxWriteService.SubscribeToSharedBoxAsync(autoFollowCommand);
            }

            await Task.WhenAll(t1, t2, t3).ConfigureAwait(false);
            var item = t2.Result;
            if (item.Type == "Link")
            {
                return Redirect(item.Source);
            }

            var nameToDownload = Path.GetFileNameWithoutExtension(item.Name);
            var extension = Path.GetExtension(item.Source);
            var url = m_BlobProviderFiles.Value.GenerateSharedAccessReadPermission(item.Source, 30,
                 ContentDispositionUtil.GetHeaderValue(nameToDownload + extension));
            return Redirect(url);
        }

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
                TraceLog.WriteError($"ChangeFileName newFileName {model.NewName} ItemUid {model.Id} userId {userId}", ex);
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
        [BoxPermission("boxId"), ActionName("Print")]
        public async Task<ActionResult> PrintAsync(long boxId, long itemId)
        {
            var userId = User.GetUserId();

            var t1 = ZboxReadService.GetItemDetailApiAsync(itemId);
            var autoFollowCommand = new SubscribeToSharedBoxCommand(userId, boxId);
            var t2 = ZboxWriteService.SubscribeToSharedBoxAsync(autoFollowCommand);
            await Task.WhenAll(t1, t2).ConfigureAwait(false);
            var item = t1.Result;
            if (item.Type == "Link")
            {
                return Redirect(item.Source);
            }

            var uri = m_BlobProviderFiles.Value.GetBlobUrl(item.Source);
            IEnumerable<string> retVal = null;
            var processor = m_FileProcessorFactory.Value.GetProcessor(uri);
            if (processor != null)
            {
                var result = await processor.ConvertFileToWebsitePreviewAsync(uri, 0).ConfigureAwait(false);
                retVal = result.Content;
            }
            return View(retVal);
        }

        [ZboxAuthorize]
        [HttpPost, ActionName("Delete")]
        public async Task<JsonResult> DeleteAsync(long itemId)
        {
            try
            {
                var command = new DeleteItemCommand(itemId, User.GetUserId());
                await ZboxWriteService.DeleteItemAsync(command).ConfigureAwait(false);
                return JsonOk(itemId);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"DeleteItem user: {User.GetUserId()}  itemId {itemId}", ex);
                return JsonError();
            }
        }

        [ZboxAuthorize, HttpPost, RemoveBoxCookie, ActionName("Like")]
        public async Task<JsonResult> LikeAsync(RateModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var id = m_GuidGenerator.Value.GetId();
                var command = new RateItemCommand(model.ItemId, User.GetUserId(), id, model.BoxId);
                await ZboxWriteService.RateItemAsync(command).ConfigureAwait(false);

                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"Rate user: {User.GetUserId()} itemId {model.ItemId}", ex);
                return JsonError();
            }
        }

        #region Preview
        [HttpGet, ActionName("Preview")]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("boxId")]
        [AsyncTimeout(TimeConst.Minute * 3 * 1000)]
        [JsonHandleError(HttpStatus = HttpStatusCode.BadRequest, ExceptionType = typeof(ArgumentException))]
        public async Task<JsonResult> PreviewAsync(string blobName, int index, long id,
            long boxId, CancellationToken cancellationToken)
        {
            Uri uri;
            if (!Uri.TryCreate(blobName, UriKind.Absolute, out uri))
            {
                uri = m_BlobProviderFiles.Value.GetBlobUrl(blobName);
            }
            try
            {
                var processor = m_FileProcessorFactory.Value.GetProcessor(uri);
                if (processor == null)
                    return
                        JsonOk(
                            new
                            {
                                template = "failed"
                        });

                var retVal = await processor.ConvertFileToWebsitePreviewAsync(uri, index, cancellationToken).ConfigureAwait(false);
                if (retVal.Content == null)
                {
                    return JsonOk(new
                    {
                        template = "failed"
                    });
                }
                if (!retVal.Content.Any())
                {
                    return JsonOk();
                }
                if (string.IsNullOrEmpty(retVal.ViewName))
                {
                    return JsonOk(new { preview = retVal.Content.First() });
                }

                return JsonOk(new
                {
                    template = retVal.ViewName,
                    retVal.Content
                    //preview = RenderRazorViewToString("_Preview" + retVal.ViewName,
                    // new ItemPreview(
                    //       retVal.Content.Take(3),
                    //       index,
                    //       User.Identity.IsAuthenticated))
                });
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"GeneratePreview filename: {blobName}", ex);
                ZboxWriteService.UpdatePreviewFailed(new PreviewFailedCommand(id));
                if (index == 0)
                {
                    return JsonOk(new
                    {
                        template = "failed"
                        //preview = RenderRazorViewToString("_PreviewFailed",
                        //    Url.RouteUrl("ItemDownload2", new { boxId, itemId = id }))
                    });
                }
                return JsonOk();
            }
        }
        #endregion

        [HttpPost]
        [ZboxAuthorize, ActionName("FlagRequest")]
        public async Task<JsonResult> FlagRequestAsync(FlagBadItem model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }

            await m_QueueProvider.InsertMessageToTransactionAsync(new BadItemData(model.BadItem.GetEnumDescription(), model.Other, User.GetUserId(), model.ItemId)).ConfigureAwait(false);
            return JsonOk();
        }

        [HttpPost, ZboxAuthorize, ActionName("AddComment")]
        public async Task<JsonResult> AddCommentAsync(NewAnnotation model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var command = new AddItemCommentCommand(model.Comment, model.ItemId, User.GetUserId(), model.BoxId);
                await ZboxWriteService.AddAnnotationAsync(command).ConfigureAwait(false);
                return JsonOk(command.CommentId);
            }
            catch (UnauthorizedAccessException)
            {
                return JsonError();
            }
        }

        [HttpPost]
        [ZboxAuthorize, ActionName("DeleteComment")]
        public async Task<JsonResult> DeleteCommentAsync(DeleteItemComment model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var command = new DeleteItemCommentCommand(model.CommentId, User.GetUserId(), model.ItemId);
            await ZboxWriteService.DeleteAnnotationAsync(command).ConfigureAwait(false);
            return JsonOk();
        }

        [HttpPost]
        [ZboxAuthorize, ActionName("ReplyComment")]
        public async Task<JsonResult> ReplyCommentAsync(ReplyItemComment model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var command = new AddItemReplyToCommentCommand(User.GetUserId(), model.ItemId, model.Comment, model.CommentId, model.BoxId);
            await ZboxWriteService.AddReplyAnnotationAsync(command).ConfigureAwait(false);
            return JsonOk(command.ReplyId);
        }

        [HttpPost, ZboxAuthorize, ActionName("DeleteCommentReply")]
        public async Task<JsonResult> DeleteCommentReplyAsync(DeleteItemCommentReply model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var command = new DeleteItemCommentReplyCommand(User.GetUserId(), model.ReplyId, model.ItemId);
            await ZboxWriteService.DeleteItemCommentReplyAsync(command).ConfigureAwait(false);
            return JsonOk();
        }

        [ZboxAuthorize, HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult UploadDialog()
        {
            return PartialView();
        }
    }
}
