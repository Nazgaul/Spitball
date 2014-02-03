using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib.Zip;
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

    public class StorageController : Controller
    {
        //Fields
        private IZboxService m_ZboxService;
        private IThumbnailProvider m_ThumbnailProvider;
        private IBlobProvider m_BlobProvider;

        //Ctor
        public StorageController(IZboxService zboxService, IThumbnailProvider thumbnailProvider, IBlobProvider blobProvider)
        {
            m_ZboxService = zboxService;
            m_ThumbnailProvider = thumbnailProvider;
            m_BlobProvider = blobProvider;
        }

        /// <summary>
        /// Getbox items - used in the unreg page
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [CompressFilter]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetBoxItemsPaged(string boxId, int pageIndex, int pageSize)
        {
            try
            {
                var BoxId = ShortCodesCache.ShortCodeToLong(boxId);
                string userEmailId = ExtractUserID.GetUserEmailId(false);

                var boxItemDtos = GetBoxItemPagedResult(BoxId, pageIndex, pageSize);

                var cachedobj = new JsonResponse(true, new { boxId = boxId, boxItemDtos = boxItemDtos });


                return this.Json(cachedobj, JsonRequestBehavior.AllowGet);
            }
            catch (BoxAccessDeniedException ex)
            {
                TraceLog.WriteInfo("Get Box items boxid " + boxId);
                TraceLog.WriteError(ex);
                return this.Json(new JsonResponse(false, "You do not have permission to view this box items"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo("Get Box items boxid " + boxId);
                TraceLog.WriteError(ex);
                return this.Json(new JsonResponse(false, "Error getting box items"), JsonRequestBehavior.AllowGet);

            }
        }

        private IList<ItemDto> GetBoxItemPagedResult(long boxId, int pageIndex, int pageSize)
        {
            GetBoxItemsPagedQuery query = new GetBoxItemsPagedQuery(boxId, ExtractUserID.GetUserEmailId(false));
            var boxItemDtos = m_ZboxService.GetBoxItemsPaged(query);
            return boxItemDtos;
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult SearchBoxItem(string searchText)
        {
            JsonResponse response = null;

            try
            {

                SearchItemByNameQuery ItemByNameQuery = new SearchItemByNameQuery(ExtractUserID.GetUserEmailId(), searchText);
                SearchItemByUploaderQuery ItemByUploaderQuery = new SearchItemByUploaderQuery(ExtractUserID.GetUserEmailId(), searchText);

                IDictionary<string, List<ItemDto>> result = m_ZboxService.SearchBoxItem(ItemByNameQuery, ItemByUploaderQuery);
                //Hibernate gives us multi element that have same reference.
                var valQueries = result.Select(s => s.Value).SelectMany(s => s).Distinct();

                foreach (ItemDto item in valQueries)
                {
                    FileDto file = item as FileDto;
                    if (file != null)
                    {
                        file.BlobUrl = RoutesCollectionZbox.GetBlobUriWihtoutHost(ShortCodesCache.LongToShortCode(file.ItemId, ShortCodesType.item));
                        file.ThumbnailBlobUrl = RoutesCollectionZbox.GetBlobUriWihtoutHost(ShortCodesCache.LongToShortCode(file.ItemId, ShortCodesType.item), eDownload.Thumbnail);
                    }
                }

                response = new JsonResponse(true, result);
            }
            catch (ArgumentException ex)
            {
                Trace.TraceInformation(string.Format("SearchBoxItem user {0} searchText {1}", this.User.Identity.Name, searchText));
                TraceLog.WriteError(ex);

                response = new JsonResponse(false, "Problem with searching your query");
            }

            return this.Json(response, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddLink(long boxId, string url, string linkComment)
        {
            try
            {
                AddLinkToBoxCommand command = new AddLinkToBoxCommand(ExtractUserID.GetUserEmailId(), boxId, url.Trim(), linkComment);
                AddLinkToBoxCommandResult result = m_ZboxService.AddLinkToBox(command);

                LinkDto dto = new LinkDto()
                {
                    ItemId = result.Link.Id,
                    Url = result.Link.Name,
                    CreationTime = result.Link.DateTimeUser.CreationTime,
                    IsUserDeleteAllowed = true,
                    Name = result.Link.Name,
                    UploaderName = "You"
                };

                CommentDto commentDto = new CommentDto()
                {
                    CommentId = result.NewComment.Id,
                    AuthorName = this.User.Identity.Name,
                    CommentText = result.NewComment.CommentText,
                    CreationTime = result.NewComment.DateTimeUser.CreationTime,
                    IsUserDeleteAllowed = true, // auther can always delete his comments

                };
                return this.Json(new JsonResponse(true, new { link = dto, comment = commentDto, boxId = boxId }));
            }
            catch (System.Data.DuplicateNameException ex)
            {
                return this.Json(new JsonResponse(false, ex.Message));
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("Problem with upload link boxID {0} url {1} user {2}", boxId, url, this.User.Identity.Name));
                TraceLog.WriteError(ex);
                return this.Json(new JsonResponse(false, "Problem with upload link"));
            }
        }


        [HttpGet]
        public FileResult DownloadAllBoxItem(long boxId)
        {
            try
            {

                var boxItems = GetBoxItemPagedResult(boxId, 0, int.MaxValue);
                MemoryStream memoryStreamZipResult = new MemoryStream();
                byte[] buffer = new byte[4096];
                ZipOutputStream zip = new ZipOutputStream(memoryStreamZipResult);
                zip.SetLevel(3);

                //check async
                foreach (var boxItem in boxItems)
                {
                    if (boxItem.CreationTime < DateTime.UtcNow.AddMinutes(2))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            FileDto file = boxItem as FileDto;
                            if (file != null)
                            {
                                var blob = m_BlobProvider.BlobContainer.GetBlobReference(file.BlobName);
                                blob.DownloadToStream(ms);
                            }
                            LinkDto link = boxItem as LinkDto;
                            if (link != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("[InternetShortcut]");
                                sb.AppendLine("URL=" + link.Url);
                                byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(sb.ToString());
                                ms.Write(bytes, 0, bytes.Length);
                                ms.Flush();


                                Uri uri = new Uri(link.Name);
                                boxItem.Name = uri.DnsSafeHost.Trim(Path.GetInvalidFileNameChars()) + ".url";

                            }
                            ms.Position = 0;
                            ZipEntry entry = new ZipEntry(ZipEntry.CleanName(boxItem.Name));
                            entry.Size = ms.Length;
                            zip.PutNextEntry(entry);
                            ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(ms, zip, buffer);
                            zip.CloseEntry();
                        }
                    }
                }
                zip.IsStreamOwner = false; // if not the zip will close the stream
                zip.Close();
                memoryStreamZipResult.Position = 0;

                FileStreamResult result = new FileStreamResult(memoryStreamZipResult, "application/zip") { FileDownloadName = "boxItems.zip" };
                return result;
            }
            catch (Exception ex)
            {

                TraceLog.WriteInfo(string.Format("DownloadAllBoxItem user {0} boxID {1}", this.User.Identity.Name, boxId));
                TraceLog.WriteError(ex);
                return null;
            }
        }


        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteItem(long ItemId, long boxId)
        {

            try
            {
                DeleteItemCommand command = new DeleteItemCommand(ItemId, ExtractUserID.GetUserEmailId(), boxId);
                DeleteItemCommandResult result = m_ZboxService.DeleteItem(command);

                return this.Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("DeleteFile fileId:{0}, userId: {1} boxID :{2}", ItemId, this.User.Identity.Name, boxId));
                TraceLog.WriteError(ex);

                return this.Json(new JsonResponse(false, "Problem with deleting file"));
            }
        }


    }


}
