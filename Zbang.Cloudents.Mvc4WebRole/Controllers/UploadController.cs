using System;
using System.Data;
using System.Data.Entity.Core;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Extensions;
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
        private readonly IBlobProvider2<ChatContainerName> m_BlobProvider2;
        private readonly IBlobProvider2<FilesContainerName> m_BlobProviderFiles;

        public UploadController(
            Lazy<IQueueProvider> queueProvider,
            IProfilePictureProvider profilePicture, ICookieHelper cookieHelper, IBlobProvider2<ChatContainerName> blobProvider2, IBlobProvider2<FilesContainerName> blobProviderFiles, IBlobProvider blobProvider)
        {
            m_BlobProvider = blobProvider;
            m_ProfilePicture = profilePicture;
            m_CookieHelper = cookieHelper;
            m_BlobProvider2 = blobProvider2;
            m_BlobProviderFiles = blobProviderFiles;
            m_BlobProvider = blobProvider;
            m_QueueProvider = queueProvider;
        }
        internal const string UploadcookieName = "upload";

        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie]
        [ActionName("File")]
        public async Task<ActionResult> FileAsync(UploadFile model)
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


                var fileUploadedDetails = GetCookieUpload(UploadcookieName, model.FileSize, model.FileName, uploadedfile);


                string blobAddressUri = fileUploadedDetails.BlobGuid.ToString().ToLower() + Path.GetExtension(fileUploadedDetails.FileName)?.ToLower();


                fileUploadedDetails.CurrentIndex = await m_BlobProviderFiles.UploadFileBlockAsync(blobAddressUri, uploadedfile.InputStream, fileUploadedDetails.CurrentIndex);
                m_CookieHelper.InjectCookie(UploadcookieName, fileUploadedDetails);

                if (!FileFinishToUpload(fileUploadedDetails))
                {
                    return JsonOk();
                }
                await m_BlobProviderFiles.CommitBlockListAsync(blobAddressUri, fileUploadedDetails.CurrentIndex, fileUploadedDetails.MimeType);
                var size = await m_BlobProviderFiles.SizeAsync(blobAddressUri);
                var command = new AddFileToBoxCommand(userId, model.BoxId, blobAddressUri,
                    fileUploadedDetails.FileName,
                     size, model.TabId, model.Comment);
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
                    OwnerId = result2.File.User.Id,
                    Source = result2.File.ItemContentUrl,
                    //Owner = result2.File.User.Name,
                    Date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),

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
                TraceLog.WriteError(
                    $"Upload UploadFileAsync BoxId {model.BoxId} fileName {model.FileName} fileSize {model.FileSize} userid {userId} ", ex);
                return JsonError(BaseControllerResources.UnspecifiedError);
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
        private FileUploadDetails GetCookieUpload(string cookieName, long fileSize, string fileName, HttpPostedFileBase uploadedfile)
        {
            var fileReceive = m_CookieHelper.ReadCookie<FileUploadDetails>(cookieName);
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


        [HttpPost, ZboxAuthorize, ActionName("ProfilePicture")]
        public async Task<JsonResult> ProfilePictureAsync()
        {
            try
            {
                if (HttpContext.Request.Files.Count == 0)
                {
                    return JsonError("no image found");
                }
                var httpPostedFileBase = HttpContext.Request.Files[0];
                if (httpPostedFileBase == null)
                {
                    return JsonError("no image found");
                }
                var result = await m_ProfilePicture.UploadProfilePicturesAsync(httpPostedFileBase.InputStream).ConfigureAwait(false);
                var command = new UpdateUserProfileImageCommand(User.GetUserId(), result.Image.AbsoluteUri);
                ZboxWriteService.UpdateUserImage(command);
                return JsonOk(result.Image.AbsoluteUri);
            }
            catch (Exception)
            {
                return JsonError("can't upload image");
            }
        }

        [HttpPost, ZboxAuthorize, ActionName("QuizImage")]
        public async Task<JsonResult> QuizImageAsync(long boxId)
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
            var url = await m_BlobProvider.UploadQuizImageAsync(file.InputStream, file.ContentType, boxId, file.FileName);
            return JsonOk(url);
        }




        internal const string ChatCookieName = "uploadchat";
        [HttpPost, ZboxAuthorize, ActionName("ChatFile")]
        public async Task<JsonResult> ChatFileAsync(UploadChatFile model)
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


            FileUploadDetails fileUploadedDetails = GetCookieUpload(ChatCookieName, model.FileSize, model.FileName, uploadedfile);


            string blobAddressUri = fileUploadedDetails.BlobGuid.ToString().ToLower() + Path.GetExtension(fileUploadedDetails.FileName)?.ToLower();


            fileUploadedDetails.CurrentIndex = await m_BlobProvider2.UploadFileBlockAsync(blobAddressUri, uploadedfile.InputStream, fileUploadedDetails.CurrentIndex);
            m_CookieHelper.InjectCookie(ChatCookieName, fileUploadedDetails);

            if (!FileFinishToUpload(fileUploadedDetails))
            {
                return JsonOk();
            }
            await m_BlobProvider2.CommitBlockListAsync(blobAddressUri, fileUploadedDetails.CurrentIndex, fileUploadedDetails.MimeType);
            var uri = m_BlobProvider2.GetBlobUrl(blobAddressUri);
            model.Users.Add(User.GetUserId());
            await m_QueueProvider.Value.InsertFileMessageAsync(new ChatFileProcessData(uri, model.Users));

            return JsonOk(blobAddressUri);
        }



        [HttpPost, ZboxAuthorize, RemoveBoxCookie, ActionName("Link")]
        public async Task<ActionResult> LinkAsync(AddLink model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            if (!model.BoxId.HasValue)
            {
                return JsonError("box id required");
            }
            try
            {
                var userid = User.GetUserId();

                var helper = new UrlTitleBringer();
                var title = model.Name;
                if (string.IsNullOrEmpty(title))
                {

                    title = await helper.BringTitleAsync(model.Url);

                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = model.Name ?? model.Url;
                }

                var command = new AddLinkToBoxCommand(userid, model.BoxId.Value, model.Url, model.TabId, title,
                    model.Question);
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
                    OwnerId = result2.Link.User.Id,
                    Source = result2.Link.ItemContentUrl,
                    //Owner = result2.Link.User.Name,
                    Date = DateTime.UtcNow,
                    //Url = result2.Link.Url,
                    //DownloadUrl =
                    //    Url.RouteUrl("ItemDownload2", new { boxId = result2.Link.Box.Id, itemId = result2.Link.Id })
                };

                if (model.TabId.HasValue)
                {
                    item.TabId = model.TabId.Value;
                }
                return JsonOk(new { item, boxId = model.BoxId });
            }
            catch (ObjectNotFoundException)
            {
                return JsonError(BoxControllerResources.LinkNotFound);
            }
            catch (DuplicateNameException)
            {
                return JsonError(BoxControllerResources.LinkExists);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"Link user: {User.GetUserId()} BoxId: {model.BoxId} url: {model.Url}", ex);
                return JsonError(BoxControllerResources.ProblemUrl);
            }
        }

        [HttpPost, ZboxAuthorize, RemoveBoxCookie, ActionName("Google")]
        public async Task<ActionResult> GooleAsync(AddLink model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            if (!model.BoxId.HasValue)
            {
                return JsonError("box id required");
            }
            try
            {
                var userid = User.GetUserId();

                using (var webRequestHandler = new WebRequestHandler { AllowAutoRedirect = false })
                {


                    using (HttpClient httpClient = new HttpClient(webRequestHandler))
                    {
                        var response = await httpClient.GetAsync(model.Url);
                        if (response.StatusCode == System.Net.HttpStatusCode.Redirect)
                        {
                            if (response.Headers.Location.AbsoluteUri.ToLowerInvariant().Contains("servicelogin"))
                            {
                                throw new ObjectNotFoundException();
                            }
                        }

                    }
                }

                var title = model.Name ?? model.Url;

                var command = new AddLinkToBoxCommand(userid, model.BoxId.Value, model.Url, model.TabId, title,
                    model.Question);
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
                    OwnerId = result2.Link.User.Id,
                    Source = result2.Link.ItemContentUrl,
                    //Owner = result2.Link.User.Name,
                    Date = DateTime.UtcNow,
                };

                if (model.TabId.HasValue)
                {
                    item.TabId = model.TabId.Value;
                }
                return JsonOk(new { item, boxId = model.BoxId });
            }
            catch (ObjectNotFoundException)
            {
                return JsonError(BoxControllerResources.GoogleShare);
            }
            catch (DuplicateNameException)
            {
                return JsonError(BoxControllerResources.LinkExists);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"Link user: {User.GetUserId()} BoxId: {model.BoxId} url: {model.Url}", ex);
                return JsonError(BoxControllerResources.ProblemUrl);
            }
        }

        [HttpPost, ZboxAuthorize, ActionName("Dropbox")]
        [RemoveBoxCookie]
        public async Task<ActionResult> DropboxAsync(AddFromDropBox model)
        {

            var userId = User.GetUserId();
            var blobAddressUri = Guid.NewGuid().ToString().ToLower() + Path.GetExtension(model.Name)?.ToLower();

            long size;
            try
            {
                await m_BlobProviderFiles.UploadFromLinkAsync(model.Url, blobAddressUri);
                size = await m_BlobProviderFiles.SizeAsync(blobAddressUri);
            }
            catch (UnauthorizedAccessException)
            {
                return JsonError();
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
                OwnerId = result2.File.User.Id,
                Source = result2.File.ItemContentUrl,
                //Owner = result2.File.User.Name,
                Date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            };
            if (model.TabId.HasValue)
            {
                fileDto.TabId = model.TabId.Value;
            }
            return JsonOk(new { item = fileDto, boxId = model.BoxId });


        }


    }
}
