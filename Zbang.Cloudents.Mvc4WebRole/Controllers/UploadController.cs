using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    public class UploadController : BaseController
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IProfilePictureProvider m_ProfilePicture;
        private readonly Lazy<IQueueProvider> m_QueueProvider;

        public UploadController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            // IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService,
            IBlobProvider blobProvider,
            Lazy<IQueueProvider> queueProvider,
            IProfilePictureProvider profilePicture)
            : base(zboxWriteService, zboxReadService,
                // shortToLongCache, 
            formsAuthenticationService)
        {
            m_BlobProvider = blobProvider;
            m_ProfilePicture = profilePicture;
            m_QueueProvider = queueProvider;
        }



        [HttpPost]
        [ZboxAuthorize]
        public async Task<ActionResult> File(long boxUid, string fileId, string fileName,
            long fileSize, Guid? tabId, string uniName, string boxName)
        {
            var userId = GetUserId();
            try
            {
                CookieHelper cookie = new CookieHelper(HttpContext);
                if (HttpContext.Request.Files == null)
                {
                    return this.CdJson(new JsonResponse(false, "No files received"));
                }
                if (HttpContext.Request.Files.Count == 0)
                {
                    return this.CdJson(new JsonResponse(false, "No files received"));
                }
                if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
                {
                    return this.CdJson(new JsonResponse(false, "No files received"));
                }
                var uploadedfile = HttpContext.Request.Files[0];


                FileUploadDetails fileUploadedDetails = GetCookieUpload(fileSize, fileName, uploadedfile);


                string blobAddressUri = fileUploadedDetails.BlobGuid.ToString().ToLower() + Path.GetExtension(fileUploadedDetails.FileName).ToLower();



                fileUploadedDetails.CurrentIndex = await m_BlobProvider.UploadFileBlockAsync(blobAddressUri, uploadedfile.InputStream, fileUploadedDetails.CurrentIndex);
                cookie.InjectCookie<FileUploadDetails>("upload", fileUploadedDetails);

                if (!FileFinishToUpload(fileUploadedDetails))
                {

                    return this.CdJson(new JsonResponse(true));
                }
                await m_BlobProvider.CommitBlockListAsync(blobAddressUri, fileUploadedDetails.CurrentIndex, fileUploadedDetails.MimeType);

                var command = new AddFileToBoxCommand(userId, boxUid, blobAddressUri,
                    fileUploadedDetails.FileName,
                     fileUploadedDetails.FileSize, tabId);
                var result = m_ZboxWriteService.AddFileToBox(command);

                var fileDto = new FileDto(result.File.Id, result.File.Name, result.File.Uploader.Id,
                    result.File.ThumbnailBlobName,
                    string.Empty, 0, 0, false, result.File.Uploader.Name);
                var urlBuilder = new UrlBuilder(HttpContext);
                fileDto.Url = urlBuilder.buildItemUrl(boxUid, boxName, result.File.Id, result.File.Name, uniName);
                cookie.RemoveCookie("upload");
                return this.CdJson(new JsonResponse(true, new { fileDto = fileDto, boxid = boxUid }));

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Upload UploadFileAsync BoxUid {0} fileId {1} fileName {2} fileSize {3} userid {4} HttpContextRequestCount {5} HttpContextRequestKeys {6}",
                    boxUid, fileId, fileName, fileSize, userId,
                    HttpContext.Request.Files.Count, string.Join(",", HttpContext.Request.Files.AllKeys)), ex);
                return this.CdJson(new JsonResponse(false, "Error"));
            }

        }




        /// <summary>
        /// Use for cookie wise
        /// </summary>
        public class FileUploadDetails
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
            CookieHelper cookie = new CookieHelper(HttpContext);
            var fileReceive = cookie.ReadCookie<FileUploadDetails>("upload");
            if (fileReceive == null) // new upload
            {
                return new FileUploadDetails
                                  {
                                      BlobGuid = Guid.NewGuid(),
                                      TotalUploadBytes = uploadedfile.ContentLength,
                                      FileSize = fileSize,
                                      FileName = fileName,
                                      //Extension = Path.GetExtension(fileName),
                                      MimeType = uploadedfile.ContentType,
                                      //BlockIds = new List<string>()
                                      CurrentIndex = 0
                                  };
            }

            if (fileName != fileReceive.FileName)
            {
                return fileReceive = new FileUploadDetails
                {
                    BlobGuid = Guid.NewGuid(),
                    TotalUploadBytes = uploadedfile.ContentLength,
                    FileSize = fileSize,
                    FileName = fileName,
                    //Extension = Path.GetExtension(fileName),
                    MimeType = uploadedfile.ContentType,
                    //BlockIds = new List<string>()
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

                UrlTitleBringer helper = new UrlTitleBringer();
                var title = model.Url;
                try
                {
                    title = await helper.BringTitle(model.Url);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("on bringing title of url " + model.Url, ex);

                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = model.Url;
                }

                var command = new AddLinkToBoxCommand(userid, model.BoxId, model.Url, model.TabId, title);
                var result = m_ZboxWriteService.AddLinkToBox(command);
                var item = new LinkDto(result.Link.Id, result.Link.Name, result.Link.Uploader.Id, result.Link.ThumbnailBlobName, string.Empty, 0, 0, false, result.Link.Uploader.Name);

                var urlBuilder = new UrlBuilder(HttpContext);
                item.Url = urlBuilder.buildItemUrl(model.BoxId, model.BoxName, item.Id, item.Name, model.UniName);

                return this.CdJson(new JsonResponse(true, item));
            }
            catch (DuplicateNameException)
            {
                return this.CdJson(new JsonResponse(false, "this link exists"));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Link user: {0} BoxUid: {1} url: {2}", GetUserId(), model.BoxId, model.Url), ex);
                return this.CdJson(new JsonResponse(false, "Problem with insert url"));
            }
        }

        [HttpPost, ZboxAuthorize]
        public async Task<ActionResult> DropBox(long boxUid, string fileUrl, string fileName, Guid? tabId, string boxName, string uniName)
        {

            var userId = GetUserId();


            var blobAddressUri = Guid.NewGuid().ToString().ToLower() + Path.GetExtension(fileName).ToLower(); ;

            var size = 0L;
            var notUploaded = true;
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
                    new UrlToDownloadData(fileUrl, fileName, boxUid, tabId, userId));
                return this.CdJson(new JsonResponse(false));
            }

            var command = new AddFileToBoxCommand(userId, boxUid, blobAddressUri,
               fileName,
                size, tabId);
            var result = m_ZboxWriteService.AddFileToBox(command);

            var fileDto = new FileDto(result.File.Id, result.File.Name, result.File.Uploader.Id,
                result.File.ThumbnailBlobName,
                string.Empty, 0, 0, false, result.File.Uploader.Name);
            var urlBuilder = new UrlBuilder(HttpContext);
            fileDto.Url = urlBuilder.buildItemUrl(boxUid, boxName, fileDto.Id, fileDto.Name, uniName);
            return this.CdJson(new JsonResponse(true, fileDto));


        }
    }
}
