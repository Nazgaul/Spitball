using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Win32;
using Zbang.Cloudents.Mobile.Controllers.Resources;
using Zbang.Cloudents.Mobile.Extensions;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Cloudents.Mobile.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class UploadController : BaseController
    {
        private readonly IBlobProvider m_BlobProvider;

        public UploadController(
            IBlobProvider blobProvider
            )
        {
            m_BlobProvider = blobProvider;
        }


        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie]
        public async Task<ActionResult> File(UploadFile model)
        {
            var userId = User.GetUserId();
            try
            {
                var cookie = new CookieHelper(HttpContext);
                if (HttpContext.Request.Files == null)
                {
                    return JsonError(BoxControllerResources.NoFilesReceived);
                }
                if (HttpContext.Request.Files.Count == 0)
                {
                    return JsonError(BoxControllerResources.NoFilesReceived);
                }
                var uploadedfile = HttpContext.Request.Files[0];
                if (uploadedfile == null)
                {
                    return JsonError(BoxControllerResources.NoFilesReceived);
                }
                var fileExtension = Path.GetExtension(model.FileName);
                if (string.IsNullOrEmpty(fileExtension))
                {
                    fileExtension = GetDefaultExtension(uploadedfile.ContentType);
                    model.FileName = model.FileName + fileExtension;
                }
                if (string.IsNullOrEmpty(Path.GetExtension(model.FileName)))
                {
                    HttpContext.Response.TrySkipIisCustomErrors = true;
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Can't upload this file");
                }
                FileUploadDetails fileUploadedDetails = GetCookieUpload(model.FileSize, model.FileName, uploadedfile);


                string blobAddressUri = fileUploadedDetails.BlobGuid.ToString().ToLower() + Path.GetExtension(fileUploadedDetails.FileName).ToLower();


                fileUploadedDetails.CurrentIndex = await m_BlobProvider.UploadFileBlockAsync(blobAddressUri, uploadedfile.InputStream, fileUploadedDetails.CurrentIndex);
                cookie.InjectCookie("upload", fileUploadedDetails);

                if (!FileFinishToUpload(fileUploadedDetails))
                {
                    return JsonOk();
                }
                await m_BlobProvider.CommitBlockListAsync(blobAddressUri, fileUploadedDetails.CurrentIndex, fileUploadedDetails.MimeType);

                var command = new AddFileToBoxCommand(userId, model.BoxId, blobAddressUri,
                    fileUploadedDetails.FileName,
                     fileUploadedDetails.TotalUploadBytes, model.TabId, model.Question);
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
                    Thumbnail = result2.File.ThumbnailUrl,
                    Owner = result2.File.Uploader.Name,
                    Date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    Url = result2.File.Url,
                    DownloadUrl = Url.RouteUrl("ItemDownload2", new { model.BoxId, itemId = result2.File.Id })
                };

                cookie.RemoveCookie("upload");
                return JsonOk(new { fileDto, boxId = model.BoxId });

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Upload UploadFileAsync BoxId {0} fileName {1} fileSize {2} userid {3} HttpContextRequestCount {4} HttpContextRequestKeys {5}",
                    model.BoxId, model.FileName, model.FileSize, userId,
                    HttpContext.Request.Files.Count, string.Join(",", HttpContext.Request.Files.AllKeys)), ex);
                return JsonError(BoxControllerResources.Error);
            }

        }

        private static string GetDefaultExtension(string mimeType)
        {
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false))
            {
                object value = key != null ? key.GetValue("Extension", null) : null;
                string result = value != null ? value.ToString() : string.Empty;

                return result;
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


        //[HttpPost, ZboxAuthorize]
        //public ActionResult ProfilePicture()
        //{
        //    //const int ImageSize = 50;
        //    //const int LargeImageSize = 100;
        //    try
        //    {
        //        if (HttpContext.Request.Files.Count == 0)
        //        {
        //            throw new Exception("No files");
        //        }
        //        var result = m_ProfilePicture.UploadProfilePictures(HttpContext.Request.Files[0].InputStream);
        //        return Json(new { Success = true, urlSmall = result.Image.AbsoluteUri, urlLarge = result.LargeImage.AbsoluteUri });
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { Success = false });
        //    }
        //}

        //[HttpPost, ZboxAuthorize]
        //[RemoveBoxCookie]
        //public async Task<ActionResult> Link(AddLink model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return JsonError(GetModelStateErrors());
        //    }
        //    try
        //    {
        //        var userid = User.GetUserId();

        //        var helper = new UrlTitleBringer();
        //        var title = model.Name;
        //        if (string.IsNullOrEmpty(title))
        //        {
        //            try
        //            {
        //                title = await helper.BringTitle(model.FileUrl);
        //            }
        //            catch (Exception ex)
        //            {
        //                TraceLog.WriteError("on bringing title of url " + model.FileUrl, ex);

        //            }
        //        }
        //        if (string.IsNullOrWhiteSpace(title))
        //        {
        //            title = model.Name;
        //        }

        //        var command = new AddLinkToBoxCommand(userid, model.BoxId, model.FileUrl, model.TabId, title, model.Question);
        //        var result = await ZboxWriteService.AddItemToBoxAsync(command);
        //        var result2 = result as AddLinkToBoxCommandResult;
        //        if (result2 == null)
        //        {
        //            throw new NullReferenceException("result2");
        //        }
        //        var item = new LinkDto(result2.Link.Id, result2.Link.Name,
        //            result2.Link.Uploader.Id,
        //            result2.Link.Uploader.Url,
        //            m_BlobProvider.GetThumbnailLinkUrl(), string.Empty,
        //            0, 0, false, result2.Link.Uploader.Name, result2.Link.ItemContentUrl, DateTime.UtcNow, result2.Link.Url)
        //         {
        //             DownloadUrl = Url.RouteUrl("ItemDownload2", new { boxId = result2.Link.Box.Id, itemId = result2.Link.Id })
        //         };
        //        return JsonOk(item);
        //    }
        //    catch (DuplicateNameException)
        //    {
        //        return JsonError(BoxControllerResources.LinkExists);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError(string.Format("Link user: {0} BoxId: {1} url: {2}", User.GetUserId(), model.BoxId, model.FileUrl), ex);
        //        return JsonError(BoxControllerResources.ProblemUrl);
        //    }
        //}

        //[HttpPost, ZboxAuthorize]
        //[RemoveBoxCookie]
        //public async Task<ActionResult> DropBox(long boxId, string fileUrl, string name, Guid? tabId)
        //{

        //    var userId = User.GetUserId();


        //    var blobAddressUri = Guid.NewGuid().ToString().ToLower() + Path.GetExtension(name).ToLower();

        //    var size = 0L;
        //    bool notUploaded;
        //    try
        //    {
        //        size = await m_BlobProvider.UploadFromLinkAsync(fileUrl, blobAddressUri);
        //        notUploaded = false;
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        notUploaded = true;
        //    }
        //    if (notUploaded)
        //    {
        //        await m_QueueProvider.Value.InsertMessageToDownloadAsync(
        //            new UrlToDownloadData(fileUrl, name, boxId, tabId, userId));
        //        return JsonOk();
        //    }
        //    var command = new AddFileToBoxCommand(userId, boxId, blobAddressUri,
        //       name,
        //        size, tabId, false);
        //    var result = await ZboxWriteService.AddItemToBoxAsync(command);
        //    var result2 = result as AddFileToBoxCommandResult;
        //    if (result2 == null)
        //    {
        //        throw new NullReferenceException("result2");
        //    }

        //    var fileDto = new FileDto(result2.File.Id, result2.File.Name, result2.File.Uploader.Id,
        //        result2.File.Uploader.Url,
        //        result2.File.ThumbnailUrl,
        //        string.Empty, 0, 0, false, result2.File.Uploader.Name, string.Empty, 0, DateTime.UtcNow, 0, result2.File.Url)
        //    {
        //        DownloadUrl = Url.RouteUrl("ItemDownload2", new { boxId, itemId = result2.File.Id })
        //    };
        //    return JsonOk(fileDto);


        //}
    }
}
