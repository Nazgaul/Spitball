using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Mvc3WebRole.Helpers;
using Zbang.Zbox.Mvc3WebRole.Models;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    public class UploadController : BaseController
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IProfilePictureProvider m_ProfilePicture;

        public UploadController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService,
            IBlobProvider blobProvider,
            IProfilePictureProvider profilePicture)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        {
            m_BlobProvider = blobProvider;
            m_ProfilePicture = profilePicture;
        }

        

        [HttpPost]
        [NoAsyncTimeout]
        [ZboxAuthorize]
        public void FileAsync(string BoxUid, string fileId, string fileName, long fileSize, string uploadId)
        {
            var userId = GetUserId();
            try
            {
                if (HttpContext.Request.Files == null || HttpContext.Request.Files.Count == 0)
                {
                    AsyncManager.Parameters["errResult"] = Json(new JsonResponse(false, "No files received"));
                    AsyncManager.Finish();
                }
                if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
                {
                    AsyncManager.Parameters["errResult"] = Json(new JsonResponse(false, "No files received"));
                    AsyncManager.Finish();
                }
                var uploadedfile = HttpContext.Request.Files[0];

                var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);


                string sessionKey = userId.ToString() + fileId;
                FileUploadDetails fileUploadedDetails = GetSessionUpload(fileSize, fileName, uploadedfile, sessionKey);
                //zoho api need the file extension in order to know which file to process
                string blobAddressUri = fileUploadedDetails.BlobGuid.ToString() + fileUploadedDetails.Extension;


                var blob = m_BlobProvider.GetFile(blobAddressUri);
                AsyncManager.Parameters["fileUploadedDetails"] = fileUploadedDetails;
                AsyncManager.Parameters["blob"] = blob;
                AsyncManager.Parameters["userId"] = userId;
                AsyncManager.Parameters["boxId"] = boxId;
                AsyncManager.Parameters["uploadId"] = uploadId;
                AsyncManager.OutstandingOperations.Increment();
                WriteFileUploadedToBlockBlob(fileUploadedDetails, uploadedfile, blob);
                Session[sessionKey] = fileUploadedDetails;

                AsyncManager.OutstandingOperations.Decrement();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Upload UploadFileAsync BoxUid {0} fileId {1} fileName {2} fileSize {3} uploadId {4} userid {5} HttpContextRequestCount {6} HttpContextRequestKeys",
                    BoxUid, fileId, fileName, fileSize, uploadId, userId,
                    HttpContext.Request.Files.Count, string.Join(",", HttpContext.Request.Files.AllKeys)), ex);
                AsyncManager.Parameters["errResult"] = Json(new JsonResponse(false, "Error"));
                AsyncManager.OutstandingOperations.Decrement();
            }

        }

        public ActionResult FileCompleted(FileUploadDetails fileUploadedDetails, CloudBlockBlob blob, long userId, long boxId, string uploadId, JsonResult errResult)
        {
            if (errResult != null)
            {
                return Json(errResult);
            }
            var jsonResult = new JsonResponse(true);
            if (!FileFinishToUpload(fileUploadedDetails))
            {
                return Json(jsonResult);
            }
            try
            {
                // Session.Remove(sessionUserDatakeyName);
                CommitBlob(fileUploadedDetails, blob, userId);

                //string thumbnailBlobAddressUri = GetFileThumbnail(fileUploadedDetails, blob);
                var command = new AddFileToBoxCommand(userId, boxId, blob.Name, fileUploadedDetails.FileName,
                     blob.Properties.Length, uploadId);
                var result = m_ZboxWriteService.AddFileToBox(command);
                //var fileDto = new FileDto(result.File.Uid, result.File.Name, result.File.UploaderId.Name, result.File.UploaderId.Uid, 0,
                //    result.File.ThumbnailBlobName, result.File.Box.Uid,
                //    result.File.Size);

                //jsonResult.Payload = fileDto;
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse(false, ex.Message));
            }
        }


        /// <summary>
        /// Use for session wise
        /// </summary>
        [Serializable]
        public class FileUploadDetails
        {
            public Guid BlobGuid { get; set; }
            public long TotalUploadBytes { get; set; }
            public long FileSize { get; set; }
            public string FileName { get; set; }
            public string FileType { get; set; }
            public string Extension { get; set; }
            public List<string> BlockIds { get; set; }
        }

        [NonAction]
        private FileUploadDetails GetSessionUpload(long fileSize, string fileName, HttpPostedFileBase uploadedfile, string sessionKey)
        {
            FileUploadDetails fileReceive;
            if (Session[sessionKey] == null) // new upload
            {
                fileReceive = new FileUploadDetails
                                  {
                                      BlobGuid = Guid.NewGuid(),
                                      TotalUploadBytes = uploadedfile.ContentLength,
                                      FileSize = fileSize,
                                      FileName = fileName,
                                      Extension = Path.GetExtension(fileName),
                                      FileType = uploadedfile.ContentType,
                                      BlockIds = new List<string>()
                                  };
            }
            else
            {
                fileReceive = Session[sessionKey] as FileUploadDetails;
                fileReceive.TotalUploadBytes += uploadedfile.ContentLength;
            }
            return fileReceive;
        }
        [NonAction]
        private void WriteFileUploadedToBlockBlob(FileUploadDetails fileUploadedDetails, HttpPostedFileBase uploadedfile, CloudBlockBlob blob)
        {
            const int blockSize = 4194304;
            const int start = 0;
            var i = 0;


            var read = new byte[blockSize];
            var count = uploadedfile.InputStream.Read(read, start, blockSize);
            while (count > 0)
            {
                var blockid = fileUploadedDetails.BlobGuid.ToString() + fileUploadedDetails.TotalUploadBytes.ToString("000000000") + i.ToString("00000");

                var blockId64String = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(blockid));
                var ms = new MemoryStream();

                ms.Write(read, start, count);
                ms.Seek(0, SeekOrigin.Begin);

                AsyncManager.OutstandingOperations.Increment();

                var tsk = Task.Factory.FromAsync<string, Stream, string>(blob.BeginPutBlock, blob.EndPutBlock,
                    blockId64String, ms, null, null);

                tsk.ContinueWith((ant) =>
                {

                    fileUploadedDetails.BlockIds.Add(blockId64String);
                    if (ms != null)
                    {
                        ms.Dispose();
                    }
                    AsyncManager.Sync(() =>
                    {
                        AsyncManager.OutstandingOperations.Decrement();
                    });

                });

                count = uploadedfile.InputStream.Read(read, start, blockSize);

                i++;
            }

        }
        [NonAction]
        private bool FileFinishToUpload(FileUploadDetails fileUploadedDetails)
        {
            return fileUploadedDetails.TotalUploadBytes == fileUploadedDetails.FileSize;
        }
        [NonAction]
        private void CommitBlob(FileUploadDetails fileUploadedDetails, CloudBlockBlob blob, long userId)
        {
            blob.PutBlockList(fileUploadedDetails.BlockIds);
            blob.Properties.ContentType = fileUploadedDetails.FileType;
            blob.Properties.CacheControl = "private max-age=" + TimeConsts.Week;

            blob.SetProperties();
           // blob.Metadata[Infrastructure.Storage.BlobProvider.blobMetadataUseridKey] = userId.ToString(CultureInfo.InvariantCulture);
            blob.SetMetadata();
            blob.FetchAttributes(); // due to later use of length i need to get the data from the blob
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
                return Json(new { Success = true, urlSmall = result.Image.AbsoluteUri , urlLarge = result.LargeImage.AbsoluteUri });
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        [HttpPost, ZboxAuthorize]
        public ActionResult Link(AddLink model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {
                var userid = GetUserId();
                var boxid = m_ShortToLongCode.ShortCodeToLong(model.BoxId);
                var command = new AddLinkToBoxCommand(userid, boxid, model.Url);
                var result = m_ZboxWriteService.AddLinkToBox(command);
                //var item = new LinkDto(result.Link.Uid, result.Link.Name, result.Link.UploaderId.Name, result.Link.UploaderId.Uid, 0, result.Link.Box.Uid);
                return Json(new JsonResponse(true, string.Empty));
            }
            catch (DuplicateNameException)
            {
                return Json(new JsonResponse(false, "this link exists"));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Link user: {0} BoxUid: {1} url: {2}", GetUserId(), model.BoxId, model.Url), ex);
                return Json(new JsonResponse(false, "Problem with insert url"));
            }
        }

        //[HttpPost, ZboxAuthorizeAttribute(Roles = ZboxRoleProvider.verifyEmailRole)]
        //public ActionResult Box(string BoxUid, string boxName)
        //{
        //    try
        //    {
        //        var userid = GetUserId();
        //        var query = new GetBoxByNameQuery(boxName, userid);
        //        var boxuid = ZboxReadService.GetUserBoxIdByName(query);
        //        //var box = userboxesList.FirstOrDefault(w => w.Name == boxName);
        //        if (string.IsNullOrWhiteSpace(boxuid))
        //        {
        //            return Json(new JsonResponse(false, "This box doesn't exist"));
        //        }

        //        var boxid = ShortToLongCode.ShortCodeToLong(BoxUid);


        //        var command = new AddLinkToBoxCommand(userid, boxid,
        //            Url.Action("Index", "Box", new RouteValueDictionary(new { boxuid = boxuid }), Request.Url.Scheme, null));
        //        var result = ZboxWriteService.AddLinkToBox(command);

        //        var item = new LinkDto(result.Link.Uid, result.Link.Name, result.Link.UploaderId.Name, result.Link.UploaderId.Uid, 0, result.Link.Box.Uid);


        //        return Json(new JsonResponse(true, item));
        //    }
        //    catch (DuplicateNameException)
        //    {
        //        return Json(new JsonResponse(false, "This box already linked"));
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError(string.Format("Box user: {0} BoxUid: {1} boxName: {2}", GetUserId(), BoxUid, boxName), ex);
        //        return Json(new JsonResponse(false, "Problem with insert box"));
        //    }
        //}
    }
}
