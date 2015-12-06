using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class UploadController : BaseController
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IProfilePictureProvider m_ProfilePicture;
        private readonly Lazy<IQueueProvider> m_QueueProvider;
        private readonly ICookieHelper m_CookieHelper;

        public UploadController(
            IBlobProvider blobProvider,
            Lazy<IQueueProvider> queueProvider,
            IProfilePictureProvider profilePicture, ICookieHelper cookieHelper)
        {
            m_BlobProvider = blobProvider;
            m_ProfilePicture = profilePicture;
            m_CookieHelper = cookieHelper;
            m_QueueProvider = queueProvider;
        }


        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie]
        public async Task<ActionResult> File(UploadFile model)
        {
            var userId = User.GetUserId();
            try
            {
                if (HttpContext.Request.Files == null)
                {
                    return JsonError(BoxControllerResources.NoFilesReceived);
                }
                if (HttpContext.Request.Files.Count == 0)
                {
                    return JsonError(BoxControllerResources.NoFilesReceived);
                }
                if (string.IsNullOrEmpty(Path.GetExtension(model.FileName)))
                {
                    return JsonError(BoxControllerResources.NoFilesReceived);
                }
                var uploadedfile = HttpContext.Request.Files[0];
                if (uploadedfile == null) throw new NullReferenceException("uploadedfile");


                FileUploadDetails fileUploadedDetails = GetCookieUpload(model.FileSize, model.FileName, uploadedfile);


                string blobAddressUri = fileUploadedDetails.BlobGuid.ToString().ToLower() + Path.GetExtension(fileUploadedDetails.FileName).ToLower();


                fileUploadedDetails.CurrentIndex = await m_BlobProvider.UploadFileBlockAsync(blobAddressUri, uploadedfile.InputStream, fileUploadedDetails.CurrentIndex);
                m_CookieHelper.InjectCookie("upload", fileUploadedDetails);

                if (!FileFinishToUpload(fileUploadedDetails))
                {
                    return JsonOk();
                }
                await m_BlobProvider.CommitBlockListAsync(blobAddressUri, fileUploadedDetails.CurrentIndex, fileUploadedDetails.MimeType);

                var command = new AddFileToBoxCommand(userId, model.BoxId, blobAddressUri,
                    fileUploadedDetails.FileName,
                     fileUploadedDetails.TotalUploadBytes, model.TabId, model.Comment);
                var result = await ZboxWriteService.AddItemToBoxAsync(command);

                var result2 = result as AddFileToBoxCommandResult;
                if (result2 == null)
                {
                    throw new NullReferenceException("result2");
                }


                var fileDto = new ItemDto
                {
                    Id = result2.File.Id,
                    Name = result2.File.Name,
                    OwnerId = result2.File.Uploader.Id,
                    UserUrl = result2.File.Uploader.Url,
                    Source = result2.File.ItemContentUrl,
                    //Thumbnail = result2.File.ThumbnailUrl,
                    Owner = result2.File.Uploader.Name,
                    Date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    Url = result2.File.Url,
                    DownloadUrl = Url.RouteUrl("ItemDownload2", new { model.BoxId, itemId = result2.File.Id }),

                };
                if (model.TabId.HasValue)
                {
                    fileDto.TabId = model.TabId.Value;
                }

                m_CookieHelper.RemoveCookie("upload");
                return JsonOk(new { item = fileDto, boxId = model.BoxId });

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Upload UploadFileAsync BoxId {0} fileName {1} fileSize {2} userid {3} HttpContextRequestCount {4} HttpContextRequestKeys {5}",
                    model.BoxId, model.FileName, model.FileSize, userId,
                    HttpContext.Request.Files.Count, string.Join(",", HttpContext.Request.Files.AllKeys)), ex);
                return JsonError(BoxControllerResources.Error);
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
            var fileReceive = m_CookieHelper.ReadCookie<FileUploadDetails>("upload");
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
        public async Task<JsonResult> ProfilePicture()
        {
            try
            {
                if (HttpContext.Request.Files.Count == 0)
                {
                    return JsonError();
                }
                var httpPostedFileBase = HttpContext.Request.Files[0];
                if (httpPostedFileBase == null)
                {
                    return JsonError();
                }
                var result = await m_ProfilePicture.UploadProfilePicturesAsync(httpPostedFileBase.InputStream);
                var command = new UpdateUserProfileImageCommand(User.GetUserId(), result.Image.AbsoluteUri);
                ZboxWriteService.UpdateUserImage(command);
                return JsonOk(result.Image.AbsoluteUri);
            }
            catch (Exception)
            {
                return JsonError();
            }
        }

        [HttpPost, ZboxAuthorize]
        public async Task<JsonResult> QuizImage(long boxId)
        {

            if (HttpContext.Request.Files == null)
            {
                return JsonError("No files");
            }
            if (HttpContext.Request.Files.Count == 0)
            {
                return JsonError("No files");
            }
            var file = HttpContext.Request.Files[0];
            if (file == null)
            {
                return JsonError("No files");
            }
            var url = await m_BlobProvider.UploadQuizImage(file.InputStream, file.ContentType, boxId, file.FileName);
            return JsonOk(url);
        }

        [HttpPost, ZboxAuthorize, RemoveBoxCookie]
        public async Task<ActionResult> Link(AddLink model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var userid = User.GetUserId();

                var helper = new UrlTitleBringer();
                var title = model.Name;
                if (string.IsNullOrEmpty(title))
                {
                    try
                    {
                        title = await helper.BringTitle(model.Url);
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("on bringing title of url " + model.Url, ex);

                    }
                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = model.Name;
                }

                var command = new AddLinkToBoxCommand(userid, model.BoxId, model.Url, model.TabId, title, model.Question);
                var result = await ZboxWriteService.AddItemToBoxAsync(command);
                var result2 = result as AddLinkToBoxCommandResult;
                if (result2 == null)
                {
                    throw new NullReferenceException("result2");
                }

                var item = new ItemDto
                {
                    Id = result2.Link.Id,
                    Name = result2.Link.Name,
                    OwnerId = result2.Link.Uploader.Id,
                    UserUrl = result2.Link.Uploader.Url,
                    Source = result2.Link.ItemContentUrl,
                    //Thumbnail = m_BlobProvider.GetThumbnailLinkUrl(),
                    Owner = result2.Link.Uploader.Name,
                    Date = DateTime.UtcNow,
                    Url = result2.Link.Url,
                    DownloadUrl = Url.RouteUrl("ItemDownload2", new { boxId = result2.Link.Box.Id, itemId = result2.Link.Id })
                };
                return JsonOk(new { item, boxId = model.BoxId });
            }
            catch (DuplicateNameException)
            {
                return JsonError(BoxControllerResources.LinkExists);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Link user: {0} BoxId: {1} url: {2}", User.GetUserId(), model.BoxId, model.Url), ex);
                return JsonError(BoxControllerResources.ProblemUrl);
            }
        }

        [HttpPost, ZboxAuthorize]
        [RemoveBoxCookie]
        public async Task<ActionResult> Dropbox(AddFromDropBox model)
        {

            var userId = User.GetUserId();


            var blobAddressUri = Guid.NewGuid().ToString().ToLower() + Path.GetExtension(model.Name).ToLower();

            var size = 0L;
            bool notUploaded;
            try
            {
                size = await m_BlobProvider.UploadFromLinkAsync(model.Url, blobAddressUri);
                notUploaded = false;
            }
            catch (UnauthorizedAccessException)
            {
                notUploaded = true;
            }
            if (notUploaded)
            {
                await m_QueueProvider.Value.InsertMessageToDownloadAsync(
                    new UrlToDownloadData(model.Url, model.Name, model.BoxId, model.TabId, userId));
                return JsonOk();
            }
            var command = new AddFileToBoxCommand(userId, model.BoxId, blobAddressUri,
               model.Name,
                size, model.TabId, model.Question);
            var result = await ZboxWriteService.AddItemToBoxAsync(command);
            var result2 = result as AddFileToBoxCommandResult;
            if (result2 == null)
            {
                throw new NullReferenceException("result2");
            }
            var fileDto = new ItemDto
            {
                Id = result2.File.Id,
                Name = result2.File.Name,
                OwnerId = result2.File.Uploader.Id,
                UserUrl = result2.File.Uploader.Url,
                Source = result2.File.ItemContentUrl,
                // Thumbnail = result2.File.ThumbnailUrl,
                Owner = result2.File.Uploader.Name,
                Date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                Url = result2.File.Url,
                DownloadUrl = Url.RouteUrl("ItemDownload2", new { model.BoxId, itemId = result2.File.Id })
            };
            return JsonOk(new { item = fileDto, boxId = model.BoxId });


        }
    }
}
