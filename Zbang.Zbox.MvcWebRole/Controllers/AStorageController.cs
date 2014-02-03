using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Security;
using Microsoft.WindowsAzure.StorageClient;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Blob;
using Zbang.Zbox.Infrastructure.Routes;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.MvcWebRole.Helpers;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.MvcWebRole.Controllers
{
    /// <summary>
    /// Async control to use heavy load
    /// </summary>
    public class AStorageController : AsyncController
    {
        private IZboxService m_ZboxService;
        private IThumbnailProvider m_ThumbnailProvider;
        private IBlobProvider m_BlobProvider;

        public AStorageController(IZboxService zboxService, IThumbnailProvider thumbnailProvider, IBlobProvider blobProvider)
        {
            m_ThumbnailProvider = thumbnailProvider;
            m_BlobProvider = blobProvider;
            m_ZboxService = zboxService;
        }

        [HttpPost]
        [Authorize]
        public JsonResult BlobSharedAccessUrl(long fileId)
        {
            const int kExpirationTime = 3;
            try
            {
                //long fileID = ShortCodes.ShortCodeToLong(fileId, eShortCodes.item);
                GetItemQuery query = new GetItemQuery(ExtractUserID.GetUserEmailId(true), fileId);
                var result = m_ZboxService.GetBoxItem(query);
                var blob = m_BlobProvider.BlobContainer.GetBlobReference(result.BlobName);

                var signedurl = blob.GetSharedAccessSignature(new SharedAccessPolicy()
                {
                    Permissions = SharedAccessPermissions.Read,
                    SharedAccessExpiryTime = DateTime.Now.AddMinutes(kExpirationTime)
                });
                var url = new Uri(blob.Uri, signedurl);

                return this.Json(new JsonResponse(true, url));
            }
            catch (BoxAccessDeniedException ex)
            {
                return this.Json(new JsonResponse(false));
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("SharedAccessUrl fileId: {0},user {1}", fileId, this.User.Identity.Name));
                TraceLog.WriteError(ex);
                return this.Json(new JsonResponse(false));
            }

        }



        [HttpGet]
        [CompressFilter]
        /*[OutputCache(Duration = 600, VaryByParam = "fileId;download")]*/
        public void DownloadBoxItemAsync(string fileId, eDownload download)
        {
            const string kDefaultMimeType = "application/octet-stream";
            long fileID = ShortCodesCache.ShortCodeToLong(fileId, ShortCodesType.item);
            GetItemQuery query = new GetItemQuery(ExtractUserID.GetUserEmailId(false), fileID);
            AsyncManager.OutstandingOperations.Increment();
            try
            {
                var result = m_ZboxService.GetBoxItem(query);
                CloudBlob blob;

                switch (download)
                {
                    case eDownload.Thumbnail:
                        blob = m_BlobProvider.BlobContainer.GetBlobReference(result.ThumbnailBlobName);
                        if (blob.Exists())
                        {
                            var contentType = kDefaultMimeType;
                            if (!string.IsNullOrWhiteSpace(blob.Properties.ContentType))
                            {
                                contentType = blob.Properties.ContentType;
                            }
                            AsyncManager.Parameters["result"] = new BlobFileStream(blob, contentType, result.Name, false);
                            AsyncManager.OutstandingOperations.Decrement();
                        }
                        else
                        {
                            goto default;
                        }
                        break;
                    case eDownload.Show:
                        if (m_ThumbnailProvider.IsImage(Path.GetExtension(result.Name)))
                        {
                            blob = m_BlobProvider.BlobContainer.GetBlobReference(result.BlobName);
                            if (blob.Exists())
                            {
                                Size sizeOfPreviewImage = new Size(500, 500);
                                MemoryStream ms = new MemoryStream();
                                blob.BeginDownloadToStream(ms, Async =>
                                {

                                    ms.Position = 0;
                                    Image smallerImage;
                                    smallerImage = m_ThumbnailProvider.ResizeImage(ms, sizeOfPreviewImage.Height, sizeOfPreviewImage.Width);
                                    ms = new MemoryStream();
                                    smallerImage.Save(ms, ImageFormat.Png);
                                    ms.Position = 0;

                                    AsyncManager.Parameters["result"] = new FileStreamResult(ms, "image/png");
                                    AsyncManager.OutstandingOperations.Decrement();
                                }, null);
                                return;
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        else
                        {
                            blob = m_BlobProvider.BlobContainer.GetBlobReference(result.ThumbnailBlobName);
                            if (blob.Exists())
                            {
                                var contentType = kDefaultMimeType;
                                if (!string.IsNullOrWhiteSpace(blob.Properties.ContentType))
                                {
                                    contentType = blob.Properties.ContentType;
                                }
                                AsyncManager.Parameters["result"] = new BlobFileStream(blob, contentType, result.Name, false);
                                AsyncManager.OutstandingOperations.Decrement();
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        break;
                    case eDownload.Download:
                        blob = m_BlobProvider.BlobContainer.GetBlobReference(result.BlobName);
                        if (blob.Exists())
                        {
                            var contentType = kDefaultMimeType;
                            if (!string.IsNullOrWhiteSpace(blob.Properties.ContentType))
                            {
                                contentType = blob.Properties.ContentType;
                            }
                            AsyncManager.Parameters["result"] = new BlobFileStream(blob, contentType, result.Name, true);
                            AsyncManager.OutstandingOperations.Decrement();
                        }
                        else
                        {
                            goto default;
                        }

                        break;
                    default:
                        AsyncManager.Parameters["result"] = new FileStreamResult(new MemoryStream(), "application/octet-stream");
                        AsyncManager.OutstandingOperations.Decrement();
                        break;
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("DownloadBoxItemAsync userid: {0}, fileId:{1}, download: {2} ", ExtractUserID.GetUserEmailId(false), fileId, download));
                TraceLog.WriteError(ex);

                AsyncManager.Parameters["result"] = new FileStreamResult(new MemoryStream(), "application/octet-stream");
                AsyncManager.OutstandingOperations.Decrement();
            }
            return;
        }

        public FileResult DownloadBoxItemCompleted(FileResult result)
        {

            return result;
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFile(long boxId, string fileid, string fileName, long fileSize, string uploadBatch, int batchSize, string userComment)
        {

            if (HttpContext.Request.Files.Count == 0)
            {
                return this.Json(new JsonResponse(false, "No files received"));
            }

            try
            {
                string userId = ExtractUserID.GetUserEmailId();

                FileUploadDetails fileUploadedDetails;
                HttpPostedFileBase uploadedfile = HttpContext.Request.Files[0];

                string sessionKey = userId.ToString() + fileid;
                fileUploadedDetails = GetSessionUpload(fileSize, fileName, userId, uploadedfile, sessionKey);

                //zoho api need the file extension in order to know which file to precess
                string blobAddressUri = fileUploadedDetails.BlobGuid.ToString() + fileUploadedDetails.Extension;

                var blob = m_BlobProvider.BlobContainer.GetBlockBlobReference(blobAddressUri);
                WriteFileUploadedToBlockBlob(fileUploadedDetails, uploadedfile, blob);

                Session.Remove(sessionKey);


                if (FileFinishToUpload(fileUploadedDetails))
                {
                    CommitBlob(fileUploadedDetails, blob);
                    string thumbnailBlobAddressUri = GetFileThumbnail(fileUploadedDetails, blob);
                    AddFileToBoxCommand command = new AddFileToBoxCommand(userId, boxId, blobAddressUri, thumbnailBlobAddressUri, fileUploadedDetails.FileName,
                         fileSize, userComment, batchSize, uploadBatch);
                    var result = m_ZboxService.AddFileToBox(command);

                    Zbox.Domain.File file = result.File as Zbox.Domain.File;

                    var fileItem = new FileDto
                      {
                          ItemId = file.Id,
                          Name = file.Name,
                          ThumbnailBlobUrl = RoutesCollectionZbox.GetBlobUriWihtoutHost(ShortCodesCache.LongToShortCode(file.Id, ShortCodesType.item), eDownload.Thumbnail),
                          BlobUrl = RoutesCollectionZbox.GetBlobUriWihtoutHost(ShortCodesCache.LongToShortCode(file.Id, ShortCodesType.item)),
                          BlobName = file.BlobName,
                          ThumbnailBlobName = file.ThumbnailBlobName,
                          CreationTime = file.DateTimeUser.CreationTime,
                          IsUserDeleteAllowed = true,
                          Size = file.Size,
                          UploaderName = "You"
                      };
                    CommentDto Boxcomment = null;
                    if (result.NewComment != null)
                    {
                        Boxcomment = new CommentDto()
                        {
                            CommentId = result.NewComment.Id,
                            AuthorName = this.User.Identity.Name,
                            CommentText = result.NewComment.CommentText,
                            CreationTime = result.NewComment.DateTimeUser.CreationTime,
                            IsUserDeleteAllowed = true, // auther can always delete his comments
                        };
                    }

                    return this.Json(new JsonResponse(true, new { fileItem = fileItem, comment = Boxcomment, boxId = boxId }));
                }
                else
                {
                    Session[sessionKey] = fileUploadedDetails;
                    return this.Json(new JsonResponse(true));
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("UploadFile boxId: {0}, fileid: {1}, fileSize {2}, user {3}", boxId, fileid, fileSize, this.User.Identity.Name));
                TraceLog.WriteError(ex);

                return this.Json(false, "Error Uploading file");


            }
        }

        private void CommitBlob(FileUploadDetails fileUploadedDetails, CloudBlockBlob blob)
        {
            blob.PutBlockList(fileUploadedDetails.blockIds);
            blob.Properties.ContentType = fileUploadedDetails.FileType;
            blob.Properties.CacheControl = "max-age=31536000";
            blob.SetProperties();
        }

        private bool FileFinishToUpload(FileUploadDetails fileUploadedDetails)
        {
            return fileUploadedDetails.TotalUploadBytes == fileUploadedDetails.FileSize;
        }

        private void WriteFileUploadedToBlockBlob(FileUploadDetails fileUploadedDetails, HttpPostedFileBase uploadedfile, CloudBlockBlob blob)
        {
            const int ThreeMB = 3145728;
            const int start = 0;
            int i = 0;

            byte[] read = new byte[ThreeMB];
            int count = uploadedfile.InputStream.Read(read, start, ThreeMB);
            while (count > 0)
            {
                string blockid = fileUploadedDetails.BlobGuid.ToString() + fileUploadedDetails.TotalUploadBytes.ToString("000000000") + i.ToString("00000");

                string blockId64String = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(blockid));

                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(read, start, count);
                    ms.Position = start;
                    blob.PutBlock(blockId64String, ms, null);
                    fileUploadedDetails.blockIds.Add(blockId64String);
                    count = uploadedfile.InputStream.Read(read, start, ThreeMB);
                }
                i++;
            }
        }

        private string GetFileThumbnail(FileUploadDetails fileReceive, CloudBlockBlob blob)
        {
            string thumbnailBlobAddressUri = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                using (MemoryStream thumbnailms = new MemoryStream())
                {
                    if (m_ThumbnailProvider.IsImage(fileReceive.Extension))
                    {
                        blob.DownloadToStream(ms);

                        ms.Position = 0;
                        //var gZipStream = new GZipStream(ms, CompressionMode.Decompress);
                        //gZipStream.CopyTo(thumbnailms);
                        ms.CopyTo(thumbnailms);
                    }

                    thumbnailBlobAddressUri = m_ThumbnailProvider.GetThumbnailBlobUrl(thumbnailms, fileReceive.BlobGuid, fileReceive.Extension, fileReceive.FileType);
                }
            }
            return thumbnailBlobAddressUri;
        }

        /// <summary>
        /// Use for session wise
        /// </summary>
        [Serializable]
        class FileUploadDetails
        {
            public Guid BlobGuid { get; set; }
            public long TotalUploadBytes { get; set; }
            public long FileSize { get; set; }
            public string FileName { get; set; }
            public string FileType { get; set; }
            public string Extension { get; set; }
            public List<string> blockIds { get; set; }
        }
        private FileUploadDetails GetSessionUpload(long fileSize, string fileName, string userId, HttpPostedFileBase uploadedfile, string sessionKey)
        {
            FileUploadDetails fileReceive;
            if (Session[sessionKey] == null) // new upload
            {
                fileReceive = new FileUploadDetails()
                {
                    BlobGuid = Guid.NewGuid(),
                    TotalUploadBytes = uploadedfile.ContentLength,
                    FileSize = fileSize,
                    FileName = fileName,
                    Extension = System.IO.Path.GetExtension(fileName),
                    FileType = uploadedfile.ContentType,
                    blockIds = new List<string>()
                };
            }
            else
            {
                fileReceive = Session[sessionKey] as FileUploadDetails;
                fileReceive.TotalUploadBytes += uploadedfile.ContentLength;
            }
            return fileReceive;
        }


        //[Authorize]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public void UploadFileAsync(long boxId, string fileid, string fileName, long fileSize, string uploadBatch, int batchSize, string userComment)
        //{
        //    const int ThreeMB = 3145728;
        //    const int start = 0;
        //    AsyncManager.OutstandingOperations.Increment();
        //    if (HttpContext.Request.Files.Count == 0)
        //    {
        //        //HttpContext.Response.ContentType = "text/plain";
        //        AsyncManager.Parameters["obejct"] = new JsonResponse(false, "No files received");
        //        AsyncManager.OutstandingOperations.Decrement();
        //        return;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            //MembershipUser membershipUser = Membership.GetUser(this.User.Identity.Name);
        //            string userId = ExtractUserID.GetUserEmailId();

        //            FileChunkDetails fileReceive;
        //            HttpPostedFileBase uploadedfile = HttpContext.Request.Files[0];

        //            string sessionKey = userId.ToString() + fileid;

        //            fileReceive = GetSessionUpload(fileSize, fileName, userId, uploadedfile, sessionKey);


        //            string blobAddressUri = fileReceive.BlobGuid.ToString();


        //            var blob = m_BlobProvider.BlobContainer.GetBlockBlobReference(blobAddressUri);
        //            int i = 0;

        //            byte[] read = new byte[ThreeMB];
        //            int count = uploadedfile.InputStream.Read(read, start, ThreeMB);
        //            while (count > 0)
        //            {
        //                string blockid = fileReceive.BlobGuid.ToString() + fileReceive.TotalUploadBytes.ToString("000000000") + i.ToString("00000");

        //                string blockId64String = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(blockid));

        //                using (MemoryStream ms = new MemoryStream())
        //                {
        //                    //using (var gZipStream = new GZipStream(ms, CompressionMode.Compress))
        //                    //{
        //                    //    gZipStream.Write(read, start, count);
        //                    ms.Write(read, start, count);
        //                    ms.Position = start;                                                     
        //                    blob.PutBlock(blockId64String, ms, null);                           
        //                    fileReceive.blockIds.Add(blockId64String);
        //                    count = uploadedfile.InputStream.Read(read, start, ThreeMB);
        //                    //}
        //                }
        //                i++;
        //            }

        //            AddFileToBoxCommandResult result = null;
        //            FileDto fileItem = null;
        //            CommentDto Boxcomment = null;
        //            if (fileReceive.TotalUploadBytes == fileReceive.FileSize)
        //            {
        //                //blob.BeginPutBlockList(fileReceive.blockIds, Async =>
        //                //{
        //                    //blob.EndPutBlockList(Async);
        //                    blob.PutBlockList(fileReceive.blockIds);
        //                    string thumbnailBlobAddressUri = GetFileThumbnail(fileReceive, blob);
        //                    AddFileToBoxCommand command = new AddFileToBoxCommand(userId, boxId, blobAddressUri, thumbnailBlobAddressUri, fileReceive.FileName,
        //                         fileSize, userComment, batchSize, uploadBatch);
        //                    result = m_ZboxService.AddFileToBox(command);


        //                    blob.Properties.ContentType = fileReceive.FileType;
        //                    blob.Properties.CacheControl = "max-age=31536000";
        //                    //blob.Properties.ContentEncoding = "gzip";
        //                    blob.BeginSetProperties(callBack => {
        //                        blob.EndSetProperties(callBack);
        //                    }, null);

        //                    Zbox.Domain.File file = result.File as Zbox.Domain.File;

        //                    fileItem = new FileDto
        //                    {
        //                        ItemId = file.Id,
        //                        Name = file.Name,
        //                        ThumbnailBlobUrl = RoutesCollectionZbox.GetBlobUriWihtoutHost(ShortCodes.LongToShortCode(file.Id, eShortCodes.item), eDownload.Thumbnail),
        //                        BlobUrl = RoutesCollectionZbox.GetBlobUriWihtoutHost(ShortCodes.LongToShortCode(file.Id, eShortCodes.item)),
        //                        BlobName = file.BlobName,
        //                        ThumbnailBlobName = file.ThumbnailBlobName,
        //                        CreationTime = file.DateTimeUser.CreationTime,
        //                        IsUserDeleteAllowed = true,
        //                        Size = file.Size,
        //                        UploaderName = "You"
        //                        //UploaderEmailId = file.UploaderId.Email
        //                    };

        //                    if (result.NewComment != null)
        //                    {
        //                        Boxcomment = new CommentDto()
        //                        {
        //                            CommentId = result.NewComment.Id,
        //                            AuthorName = this.User.Identity.Name,
        //                            CommentText = result.NewComment.CommentText,
        //                            CreationTime = result.NewComment.DateTimeUser.CreationTime.ToString(),
        //                            IsUserDeleteAllowed = true, // auther can always delete his comments

        //                        };
        //                    }

        //                    Session.Remove(sessionKey);
        //              //  }, null);
        //            }
        //            else
        //            {
        //                Session[sessionKey] = fileReceive;
        //            }



        //            AsyncManager.Parameters["obejct"] = new JsonResponse(true, new { fileItem = fileItem, comment = Boxcomment, boxId = boxId });
        //            AsyncManager.OutstandingOperations.Decrement();
        //            return;
        //            //return this.Json(new JsonResponse(true, new { fileItem = fileItem, comment = Boxcomment, boxId = boxId }));
        //        }
        //        catch (Exception ex)
        //        {
        //            TraceLog.WriteInfo(string.Format("UploadFile boxId: {0}, fileid: {1}, fileSize {2}, user {3}", boxId, fileid, fileSize, this.User.Identity.Name));
        //            TraceLog.WriteError(ex);

        //            //return this.Json(false, "Error Uploading file");
        //            AsyncManager.Parameters["obejct"] = new JsonResponse(false, "Error Uploading file");
        //            AsyncManager.OutstandingOperations.Decrement();
        //            return;

        //        }
        //    }
        //}


        //public JsonResult UploadFileCompleted(JsonResponse obejct)
        //{
        //    return this.Json(obejct);
        //}



    }


}
