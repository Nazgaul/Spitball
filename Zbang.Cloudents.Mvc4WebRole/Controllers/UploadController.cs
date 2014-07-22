using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    public class UploadController : BaseController
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IProfilePictureProvider m_ProfilePicture;
        private readonly Lazy<IQueueProvider> m_QueueProvider;

        public UploadController(
            IBlobProvider blobProvider,
            Lazy<IQueueProvider> queueProvider,
            IProfilePictureProvider profilePicture)
        {
            m_BlobProvider = blobProvider;
            m_ProfilePicture = profilePicture;
            m_QueueProvider = queueProvider;
        }


        [HttpPost]
        [ZboxAuthorize]
        public async Task<ActionResult> File(long boxId, string fileName,
            long fileSize, Guid? tabId)
        {
            var userId = GetUserId();
            try
            {
                var cookie = new CookieHelper(HttpContext);
                if (HttpContext.Request.Files == null)
                {
                    return this.CdJson(new JsonResponse(false, BaseControllerResources.NoFilesReceived));
                }
                if (HttpContext.Request.Files.Count == 0)
                {
                    return this.CdJson(new JsonResponse(false, BaseControllerResources.NoFilesReceived));
                }
                if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
                {
                    return this.CdJson(new JsonResponse(false, BaseControllerResources.NoFilesReceived));
                }
                var uploadedfile = HttpContext.Request.Files[0];


                FileUploadDetails fileUploadedDetails = GetCookieUpload(fileSize, fileName, uploadedfile);


                string blobAddressUri = fileUploadedDetails.BlobGuid.ToString().ToLower() + Path.GetExtension(fileUploadedDetails.FileName).ToLower();



                fileUploadedDetails.CurrentIndex = await m_BlobProvider.UploadFileBlockAsync(blobAddressUri, uploadedfile.InputStream, fileUploadedDetails.CurrentIndex);
                cookie.InjectCookie("upload", fileUploadedDetails);

                if (!FileFinishToUpload(fileUploadedDetails))
                {

                    return this.CdJson(new JsonResponse(true));
                }
                await m_BlobProvider.CommitBlockListAsync(blobAddressUri, fileUploadedDetails.CurrentIndex, fileUploadedDetails.MimeType);

                var command = new AddFileToBoxCommand(userId, boxId, blobAddressUri,
                    fileUploadedDetails.FileName,
                     fileUploadedDetails.FileSize, tabId);
                var result = ZboxWriteService.AddFileToBox(command);

                var urlBuilder = new UrlBuilder(HttpContext);
                var fileDto = new FileDto(result.File.Id, result.File.Name, result.File.Uploader.Id,
                    result.File.Uploader.Url,
                    result.File.ThumbnailUrl,
                    string.Empty, 0, 0, false, result.File.Uploader.Name, string.Empty, 0, DateTime.UtcNow, 0,
                    result.File.Url)
                {
                    DownloadUrl = urlBuilder.BuildDownloadUrl(result.File.Box.Id, result.File.Id)
                };

                cookie.RemoveCookie("upload");
                return this.CdJson(new JsonResponse(true, new { fileDto, boxid = boxId }));

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Upload UploadFileAsync BoxUid {0} fileName {1} fileSize {2} userid {3} HttpContextRequestCount {4} HttpContextRequestKeys {5}",
                    boxId, fileName, fileSize, userId,
                    HttpContext.Request.Files.Count, string.Join(",", HttpContext.Request.Files.AllKeys)), ex);
                return this.CdJson(new JsonResponse(false, BaseControllerResources.Error));
            }

        }




        /// <summary>
        /// Use for cookie wise
        /// </summary>
        private class FileUploadDetails
        {
            public Guid BlobGuid { get; set; }
            public long TotalUploadBytes { get; set; }
            public long FileSize { get; set; }
            public string FileName { get; set; }
            public string MimeType { get; set; }
            public int CurrentIndex { get; set; }
        }

        [NonAction]
        private FileUploadDetails GetCookieUpload(long fileSize, string fileName, HttpPostedFileBase uploadedfile)
        {
            //FileUploadDetails fileReceive;
            var cookie = new CookieHelper(HttpContext);
            var fileReceive = cookie.ReadCookie<FileUploadDetails>("upload");
            if (fileReceive != null && fileReceive.FileSize <= fileReceive.TotalUploadBytes)
            {
                fileReceive = null;
            }
            if (fileReceive == null) // new upload
            {
                return new FileUploadDetails
                                  {
                                      BlobGuid = Guid.NewGuid(),
                                      TotalUploadBytes = uploadedfile.ContentLength,
                                      FileSize = fileSize,
                                      FileName = fileName,
                                      MimeType = uploadedfile.ContentType,
                                      CurrentIndex = 0
                                  };
            }

            if (fileName != fileReceive.FileName)
            {
                return new FileUploadDetails
                {
                    BlobGuid = Guid.NewGuid(),
                    TotalUploadBytes = uploadedfile.ContentLength,
                    FileSize = fileSize,
                    FileName = fileName,
                    MimeType = uploadedfile.ContentType,
                    CurrentIndex = 0
                };
            }
            fileReceive.TotalUploadBytes += uploadedfile.ContentLength;
            return fileReceive;

        }

        [NonAction]
        private bool FileFinishToUpload(FileUploadDetails fileUploadedDetails)
        {
            return fileUploadedDetails.TotalUploadBytes == fileUploadedDetails.FileSize;
        }


        [HttpPost, ZboxAuthorize]
        public ActionResult ProfilePicture()
        {
            //const int ImageSize = 50;
            //const int LargeImageSize = 100;
            try
            {
                if (HttpContext.Request.Files.Count == 0)
                {
                    throw new Exception("No files");
                }
                var result = m_ProfilePicture.UploadProfilePictures(HttpContext.Request.Files[0].InputStream);
                return Json(new { Success = true, urlSmall = result.Image.AbsoluteUri, urlLarge = result.LargeImage.AbsoluteUri });
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        [HttpPost, ZboxAuthorize]
        public async Task<ActionResult> Link(AddLink model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {
                var userid = GetUserId();

                var helper = new UrlTitleBringer();
                var title = model.FileUrl;
                try
                {
                    title = await helper.BringTitle(model.FileUrl);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("on bringing title of url " + model.FileUrl, ex);

                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = model.FileUrl;
                }

                var command = new AddLinkToBoxCommand(userid, model.BoxId, model.FileUrl, model.TabId, title);
                var result = ZboxWriteService.AddLinkToBox(command);
                var urlBuilder = new UrlBuilder(HttpContext);

                var item = new LinkDto(result.Link.Id, result.Link.Name,
                    result.Link.Uploader.Id,
                    result.Link.Uploader.Url,
                    m_BlobProvider.GetThumbnailLinkUrl(), string.Empty,
                    0, 0, false, result.Link.Uploader.Name, result.Link.ItemContentUrl, DateTime.UtcNow, result.Link.Url)
                 {
                     DownloadUrl = urlBuilder.BuildDownloadUrl(result.Link.Box.Id, result.Link.Id)
                };
                return this.CdJson(new JsonResponse(true, item));
            }
            catch (DuplicateNameException)
            {
                return this.CdJson(new JsonResponse(false, BaseControllerResources.LinkExists));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Link user: {0} BoxUid: {1} url: {2}", GetUserId(), model.BoxId, model.FileUrl), ex);
                return this.CdJson(new JsonResponse(false, BaseControllerResources.ProblemUrl));
            }
        }

        [HttpPost, ZboxAuthorize]
        public async Task<ActionResult> DropBox(long boxId, string fileUrl, string fileName, Guid? tabId)
        {

            var userId = GetUserId();


            var blobAddressUri = Guid.NewGuid().ToString().ToLower() + Path.GetExtension(fileName).ToLower();

            var size = 0L;
            bool notUploaded;
            try
            {
                size = await m_BlobProvider.UploadFromLinkAsync(fileUrl, blobAddressUri);
                notUploaded = false;
            }
            catch (UnauthorizedAccessException)
            {
                notUploaded = true;
            }
            if (notUploaded)
            {
                await m_QueueProvider.Value.InsertMessageToDownloadAsync(
                    new UrlToDownloadData(fileUrl, fileName, boxId, tabId, userId));
                return this.CdJson(new JsonResponse(true));
            }
            var urlBuilder = new UrlBuilder(HttpContext);
            var command = new AddFileToBoxCommand(userId, boxId, blobAddressUri,
               fileName,
                size, tabId);
            var result = ZboxWriteService.AddFileToBox(command);
            var fileDto = new FileDto(result.File.Id, result.File.Name, result.File.Uploader.Id,
                result.File.Uploader.Url,
                result.File.ThumbnailUrl,
                string.Empty, 0, 0, false, result.File.Uploader.Name, string.Empty, 0, DateTime.UtcNow, 0, result.File.Url)
            {
                DownloadUrl = urlBuilder.BuildDownloadUrl(result.File.Box.Id, result.File.Id)
            };
            return this.CdJson(new JsonResponse(true, fileDto));


        }
    }
}
