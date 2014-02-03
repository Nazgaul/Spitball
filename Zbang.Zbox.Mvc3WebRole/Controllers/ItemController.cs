using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.FileConvert;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Mvc3WebRole.Helpers;
using Zbang.Zbox.Mvc3WebRole.Models;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ItemController : BaseController
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IThumbnailProvider m_ThumbnailProvider;
        private readonly IFileConvertFactory m_ConvertFactory;

        public ItemController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService,
            IBlobProvider blobProvider,
            IThumbnailProvider thumbnailProvider,
            IFileConvertFactory convertFactory)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        {
            m_BlobProvider = blobProvider;
            m_ThumbnailProvider = thumbnailProvider;
            m_ConvertFactory = convertFactory;
        }


       


        /// <summary>
        /// Item Page
        /// </summary>
        /// <param name="BoxUid"></param>
        /// <param name="ItemUid"></param>
        /// <returns></returns>
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult Index(string BoxUid, string ItemUid)
        {
            try
            {
                if (string.IsNullOrEmpty(BoxUid) || string.IsNullOrEmpty(ItemUid))
                    return RedirectToAction("Index", "Dashboard");

                var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
                var userId = GetUserId(false); // not really needs it
                var itemId = m_ShortToLongCode.ShortCodeToLong(ItemUid, ShortCodesType.Item);
                ItemView model = GenerateItemView(boxId, userId, itemId);
                ViewBag.boxid = BoxUid;
                return View("Index2", model);
            }
            catch (BoxAccessDeniedException)
            {
                return View("Error");
            }
            catch (BoxDoesntExistException)
            {
                return View("Error");
            }
            catch (Exception ex)
            {
                var userId = GetUserId(false);
                TraceLog.WriteError(string.Format("Item Index BoxUid {0} userid {1} ItemUid {2}", BoxUid, userId, ItemUid), ex);
                return View("Error");
            }
        }

        /// <summary>
        /// Ajax Request - user press next prev icon on item page
        /// </summary>
        /// <param name="BoxUid"></param>
        /// <param name="ItemUid"></param>
        /// <returns></returns>
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpPost]
        public ActionResult Load(string BoxUid, string ItemUid)
        {
            if (string.IsNullOrEmpty(BoxUid) || string.IsNullOrEmpty(ItemUid))
                return Json(new JsonResponse(false));
            var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var userId = GetUserId(false); // not really needs it
            var itemId = m_ShortToLongCode.ShortCodeToLong(ItemUid, ShortCodesType.Item);

            ItemView model = GenerateItemView(boxId, userId, itemId);
            return Json(new JsonResponse(true, model));
        }

        /// <summary>
        /// Download Item
        /// </summary>
        /// <param name="BoxUid"></param>
        /// <param name="ItemUid"></param>
        /// <returns>if link redirect to link if file download</returns>
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult Download(string BoxUid, string ItemUid)
        {
            const string DefaultMimeType = "application/octet-stream";

            var itemId = m_ShortToLongCode.ShortCodeToLong(ItemUid, ShortCodesType.Item);
            var boxid = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var query = new GetItemQuery(GetUserId(false), itemId, boxid);

            var item = m_ZboxReadService.GetItem(query);


            var filedto = item as FileWithDetailDto;
            if (filedto == null) // link
            {
                return Redirect(item.Name);
            }
            var blob = m_BlobProvider.GetFile(filedto.Blob);
            if (!blob.Exists())
            {
                throw new StorageClientException();
            }

            var contentType = DefaultMimeType;
            if (!string.IsNullOrWhiteSpace(blob.Properties.ContentType))
            {
                contentType = blob.Properties.ContentType;
            }
            return new BlobFileStream(blob, contentType, item.Name, true);
        }

        /// <summary>
        /// Used to rename file name - item name cannot be changed
        /// </summary>
        /// <param name="newFileName"></param>
        /// <param name="ItemUid"></param>
        /// <returns></returns>
        [ZboxAuthorize]
        [HttpPost]
        public JsonResult Rename(string newFileName, string ItemUid)
        {
            if (newFileName.Length > Item.NameLength)
            {
                return Json(new JsonResponse(false, "File name to long"));
            }
            var itemid = m_ShortToLongCode.ShortCodeToLong(ItemUid, ShortCodesType.Item);
            var userId = GetUserId();
            try
            {
                var command = new ChangeFileNameCommand(itemid, newFileName, userId);
                var result = m_ZboxWriteService.ChangeFileName(command);
                return Json(new JsonResponse(true, result.File.Name));
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
                TraceLog.WriteError(string.Format("ChangeFileName newFileName {0} ItemUid {1} userId {2}", newFileName, ItemUid, userId), ex);
                return Json(new JsonResponse(false, "Error"));
            }


        }

        /// <summary>
        /// Print selected file
        /// </summary>
        /// <param name="BoxUid"></param>
        /// <param name="ItemUid"></param>
        /// <returns>View with no layout and print command in javascript</returns>
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult Print(string BoxUid, string ItemUid)
        {
            var itemId = m_ShortToLongCode.ShortCodeToLong(ItemUid, ShortCodesType.Item);
            var boxid = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var userId = GetUserId(false);
            var model = GenerateItemView(boxid, userId, itemId);

            return View(model);

        }

        /// <summary>
        /// Generate Item Preview
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        /// <param name="widthScreenSize">The screen width of the user - null if you want the default size</param>
        /// <returns></returns>
        [NonAction]
        private ItemView GenerateItemView(long boxId, long userId, long itemId, int? widthScreenSize = null, int? heightScrrenSize = null)
        {
            var query = new GetItemQuery(userId, itemId, boxId);
            var item = m_ZboxReadService.GetItem(query);
            ItemView model = new ItemView(item, Preview(item, widthScreenSize, heightScrrenSize));
            return model;
        }

        [ZboxAuthorize]
        [HttpPost]
        public ActionResult Delete(string ItemUid, string BoxUid)
        {
            try
            {
                var itemId = m_ShortToLongCode.ShortCodeToLong(ItemUid, ShortCodesType.Item);
                var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
                var userEmailId = GetUserId(false);

                var command = new DeleteItemCommand(itemId, userEmailId, boxId);
                m_ZboxWriteService.DeleteItem(command);

                return Json(new JsonResponse(true, ItemUid));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteItem user: {0} boxid: {1} itemId {2}", GetUserId(), BoxUid, ItemUid), ex);
                return Json(new JsonResponse(false));
            }
        }


        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpPost]
        public ActionResult Enlarge(string ItemUid, string BoxUid, int width, int height)
        {
            var itemId = m_ShortToLongCode.ShortCodeToLong(ItemUid, ShortCodesType.Item);
            var boxid = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var userId = GetUserId(false);
            var model = GenerateItemView(boxid, userId, itemId, width, height);
            return Json(new JsonResponse(true, model));
        }

        #region Preview
        [NonAction]
        public string Preview(ItemWithDetailDto dto, int? widthScreenSize, int? heightScrrenSize)
        {
            var filedto = dto as FileWithDetailDto;
            if (filedto == null) // link
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(dto.Name, "/Box\\?BoxUid=.+", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return RenderRazorViewToString("_PreviewBox", dto);
                }
                return RenderRazorViewToString("_PreviewLink", dto.Name);
            }
            return FilePreview(filedto.Blob, dto.BoxUid, dto.Uid, widthScreenSize, heightScrrenSize);
        }


        [NonAction]
        private string FilePreview(string blobName, string boxid, string itemid, int? widthScreenSize, int? heightScrrenSize)
        {
            var blob = m_BlobProvider.GetFile(blobName);
            try
            {
                var mimeType = m_BlobProvider.FetchBlobMimeType(blobName);
                if (mimeType.StartsWith("image"))
                {
                    var fileImagePreview = new ImagePreview(m_BlobProvider, m_ThumbnailProvider);
                    var previewUrl = fileImagePreview.GenerateImagePreview(blobName, ZboxSize.GenerateSize(widthScreenSize, heightScrrenSize));
                    var blobSharedUrl = m_BlobProvider.GetFileUrlInCahce(previewUrl);
                    return RenderRazorViewToString("_PreviewImage", blobSharedUrl);
                    // return Json(new JsonResponse(true, new { Type = "img", data = blobSharedUrl }));
                }

                if (mimeType.StartsWith("text"))
                {
                    return GenerateDocument(blob);
                }
                if (mimeType.StartsWith("application/pdf"))
                {
                    return GenerateDocument(blob);
                }
                //if (SupportWithMediaPlayer(mimeType))
                //{
                //    return Json(new JsonResponse(true, new { Type = "media", data = GenerateSharedAccessUrl(blob) }));
                //}

                if (CanConvertToDocument(blobName))
                {
                    return ConvertToPdf(blobName);
                }
                return RenderRazorViewToString("_PreviewFailed", Url.Action("Download", new { BoxUid = boxid, ItemUid = itemid }));
                //return Json(new JsonResponse(true, new { Type = "nopreview" }));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("GeneratePreview filename: {0}", blobName), ex);
                return RenderRazorViewToString("_PreviewFailed", Url.Action("Download", new { BoxUid = boxid, ItemUid = itemid }));
                //return Json(new JsonResponse(true, new { Type = "nopreview" }));
            }
        }

        [NonAction]
        private bool SupportWithMediaPlayer(string mimeType)
        {
            var listOfFormat = new List<string>
            {
                "video/webm","video/mp4","video/x-flv","video/ogg",
                "audio/aac", "audio/aacp", "audio/3gpp", "audio/3gpp2", "audio/mp4", "audio/mp4a-latm", "audio/mpeg4-generic", "audio/ogg", "audio/vorbis", "audio/vorbis-config","audio/mpeg","audio/mpa","audio/mpa-robust","audio/mp3"
            };
            return listOfFormat.Exists(s => s == mimeType);
        }
        [NonAction]
        private string GenerateSharedAccessUrl(CloudBlob blob)
        {
            return m_BlobProvider.GenerateSharedAccessReadPermissionBlobFiles(blob, 10);
        }
        [NonAction]
        private string GenerateDocument(CloudBlob blob)
        {
            return RenderRazorViewToString("_PreviewLink", GenerateSharedAccessUrl(blob));
            //return Json(new JsonResponse(true, new { Type = "link", data = GenerateSharedAccessUrl(blob) }));
        }
        [NonAction]
        private bool CanConvertToDocument(string p)
        {
            return m_ConvertFactory.CanConvertFile(p);
        }
        [NonAction]
        private string ConvertToPdf(string blobName)
        {
            //var blob = BlobProvider.ZboxCacheContainerFile(blobName);
            if (!m_BlobProvider.CheckIfFileExistsInCache(blobName))
            {
                var convertor = m_ConvertFactory.GetConvertor(blobName);
                convertor.ConvertFileToPdf();
            }
            var blobSharedUrl = m_BlobProvider.GetFileUrlInCahce(blobName);
            return RenderRazorViewToString("_PreviewLink", blobSharedUrl);
            //return Json(new JsonResponse(true, new { Type = "link", data = blobSharedUrl }));
        }
        #endregion
    }
}
