using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
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
        [NoAsyncTimeout]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public void DownloadAllAsync(string BoxUid)
        {
            const int oemHebrewCode = 862;

            const string licenseKey = "IDANAPZIP_2YG9b4GVlPyH";
            var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var query = new GetBoxItemsPagedQuery(boxId, GetUserId(false));
            var boxItems = m_ZboxReadService.GetBoxItemsPaged(query);
            var zip = new Chilkat.Zip();
            zip.UnlockComponent(licenseKey);
            zip.NewZip("test.zip");
            zip.OemCodePage = oemHebrewCode;
            var boxName = string.Empty;
            AsyncManager.OutstandingOperations.Increment();

            try
            {
                foreach (var boxItem in boxItems.Dto)
                {
                    var itemName = boxItem.Name;
                    try
                    {
                        if (boxItem.CreationTime > DateTime.UtcNow.AddMinutes(2))
                        {
                            continue;
                        }
                        byte[] bytes = null;

                        var file = boxItem as FileDto;
                        if (file != null)
                        {
                            bytes = m_BlobProvider.DownloadFileToBytes(file.BlobName);
                        }
                        var link = boxItem as LinkDto;
                        if (link != null)
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("[InternetShortcut]");
                            sb.AppendLine("URL=" + link.Name);
                            bytes = Encoding.ASCII.GetBytes(sb.ToString());


                            var uri = new Uri(link.Name);
                            itemName = uri.DnsSafeHost.Trim(Path.GetInvalidFileNameChars()) + ".url";

                        }

                        zip.AppendData(itemName, bytes);

                        if (string.IsNullOrEmpty(boxName))
                            boxName = boxItem.BoxName;
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("DownloadAllAsync boxItemuid: " + boxItem.Uid, ex);
                        throw;
                    }

                }

                byte[] zipFile = zip.WriteToMemory();


                AsyncManager.Parameters["ms"] = new MemoryStream(zipFile);
                AsyncManager.Parameters["boxName"] = boxName;
                AsyncManager.OutstandingOperations.Decrement();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("DownloadAllAsync", ex);
                AsyncManager.Parameters["ms"] = new MemoryStream();
                AsyncManager.Parameters["boxName"] = "Error";
                AsyncManager.OutstandingOperations.Decrement();
            }
        }

        public ActionResult DownloadAllCompleted(MemoryStream ms, string boxName)
        {
            return new FileStreamResult(ms, "application/zip") { FileDownloadName = boxName + ".zip" };
        }

        

        


    }
}
