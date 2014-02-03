using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class FileController : BaseController
    {
        public FileController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService,
            IBlobProvider blobProvider)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        {
            m_BlobProvider = blobProvider;
        }

        private readonly IBlobProvider m_BlobProvider;

        //TODO: Change to box item paged 2
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public async Task<ActionResult> DownloadAll(string boxUid)
        {
            var boxId = m_ShortToLongCode.ShortCodeToLong(boxUid);
            var query = new GetBoxItemsPagedQuery(boxId, GetUserId(false));
            var boxItems = m_ZboxReadService.GetBoxItemsPaged(query);

            var boxName = string.Empty;
            var ms = new MemoryStream(); //FileStreamResult dispose ms

            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach (var boxItem in boxItems.Dto)
                {
                    boxName = boxItem.BoxName;
                    if (boxItem.CreationTime > DateTime.UtcNow.AddMinutes(2))
                    {
                        continue;
                    }
                    var file = boxItem as FileDto;
                    if (file != null)
                    {
                        using (var stream = await m_BlobProvider.DownloadFileAsync(file.BlobName))
                        {
                            var entry = zip.CreateEntry(file.Name, CompressionLevel.Optimal);
                            using (var sr = entry.Open())
                            {
                                await stream.CopyToAsync(sr);
                            }
                        }
                        //bytes = m_BlobProvider.DownloadFileToBytes(file.BlobName);
                    }
                    var link = boxItem as LinkDto;
                    if (link != null)
                    {
                        var sb = new StringBuilder();
                        //sb.AppendLine("[InternetShortcut]");
                        //sb.AppendLine("URL=" + link.Name);
                        //var bytes = Encoding.ASCII.GetBytes(sb.ToString());


                        var uri = new Uri(link.Name);
                        var itemName = uri.DnsSafeHost.Trim(Path.GetInvalidFileNameChars()) + ".url";

                        var entry = zip.CreateEntry(itemName, CompressionLevel.Optimal);
                        using (var sr = new StreamWriter(entry.Open()))
                        {
                            sr.WriteLine("[InternetShortcut]");
                            sr.WriteLine("URL=" + link.Name);
                        }
                    }
                }
            }
            ms.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(ms, "application/zip") { FileDownloadName = boxName + ".zip" };

        }


        //[NoAsyncTimeout]
        //[ZboxAuthorize(IsAuthenticationRequired = false)]
        //public void DownloadAllAsync(string boxUid)
        //{
        //    const int oemHebrewCode = 862;

        //    const string licenseKey = "IDANAPZIP_2YG9b4GVlPyH";
        //    var boxId = m_ShortToLongCode.ShortCodeToLong(boxUid);
        //    var query = new GetBoxItemsPagedQuery(boxId, GetUserId(false));
        //    var boxItems = m_ZboxReadService.GetBoxItemsPaged(query);
        //    var zip = new Chilkat.Zip();
        //    zip.UnlockComponent(licenseKey);
        //    zip.NewZip("test.zip");
        //    zip.OemCodePage = oemHebrewCode;
        //    var boxName = string.Empty;
        //    AsyncManager.OutstandingOperations.Increment();

        //    try
        //    {
        //        foreach (var boxItem in boxItems.Dto)
        //        {

        //            var itemName = boxItem.Name;
        //            try
        //            {
        //                if (boxItem.CreationTime > DateTime.UtcNow.AddMinutes(2))
        //                {
        //                    continue;
        //                }
        //                byte[] bytes = null;

        //                var file = boxItem as FileDto;
        //                if (file != null)
        //                {
        //                    bytes = m_BlobProvider.DownloadFileToBytes(file.BlobName);
        //                }
        //                var link = boxItem as LinkDto;
        //                if (link != null)
        //                {
        //                    var sb = new StringBuilder();
        //                    sb.AppendLine("[InternetShortcut]");
        //                    sb.AppendLine("URL=" + link.Name);
        //                    bytes = Encoding.ASCII.GetBytes(sb.ToString());


        //                    var uri = new Uri(link.Name);
        //                    itemName = uri.DnsSafeHost.Trim(Path.GetInvalidFileNameChars()) + ".url";

        //                }

        //                zip.AppendData(itemName, bytes);

        //                if (string.IsNullOrEmpty(boxName))
        //                    boxName = boxItem.BoxName;
        //            }
        //            catch (Exception ex)
        //            {
        //                TraceLog.WriteError("DownloadAllAsync boxItemuid: " + boxItem.Uid, ex);
        //                throw;
        //            }

        //        }

        //        byte[] zipFile = zip.WriteToMemory();


        //        AsyncManager.Parameters["ms"] = new MemoryStream(zipFile);
        //        AsyncManager.Parameters["boxName"] = boxName;
        //        AsyncManager.OutstandingOperations.Decrement();
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("DownloadAllAsync", ex);
        //        AsyncManager.Parameters["ms"] = new MemoryStream();
        //        AsyncManager.Parameters["boxName"] = "Error";
        //        AsyncManager.OutstandingOperations.Decrement();
        //    }
        //}

        //public ActionResult DownloadAllCompleted(MemoryStream ms, string boxName)
        //{
        //    return new FileStreamResult(ms, "application/zip") { FileDownloadName = boxName + ".zip" };
        //}






    }
}
