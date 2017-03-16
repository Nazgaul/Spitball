using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc2Jared.Models;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries.Jared;
using System.Threading;
using Zbang.Zbox.ReadServices;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Aspose.Cells;
using System.Net;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Cloudents.Mvc2Jared.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : Controller
    {
        private readonly IZboxCacheReadService m_readService;
        private readonly Lazy<IFileProcessorFactory> m_FileProcessorFactory;
        private readonly Lazy<IBlobProvider2<FilesContainerName>> m_BlobProviderFiles;
        public HomeController(IZboxCacheReadService readService,
            Lazy<IBlobProvider2<FilesContainerName>> blobProviderFiles,
            Lazy<IFileProcessorFactory> fileProcessorFactory)
        {
            m_readService = readService;
            m_BlobProviderFiles = blobProviderFiles;
            m_FileProcessorFactory = fileProcessorFactory;
        }

        public ActionResult Page()
        {
            return View();
        }
     
        [HttpPost, ActionName("Items")]
        public async Task<JsonResult> ItemsAsync(JaredSearchQuery model, CancellationToken cancellationToken)
        {
           var retVal = await m_readService.GetItemsWithTagsAsync(model);
           return Json(retVal);
        }
        #region Preview
        [HttpGet, ActionName("Preview")]
        [AsyncTimeout(TimeConst.Minute * 3 * 1000)]
        public async Task<JsonResult> PreviewAsync(string blobName, long id,CancellationToken cancellationToken)
        {
            int index = 5;
            Uri uri;
            if (!Uri.TryCreate(blobName, UriKind.Absolute, out uri))
            {
                uri = m_BlobProviderFiles.Value.GetBlobUrl(blobName);
            }
            try
            {
                var processor = m_FileProcessorFactory.Value.GetProcessor(uri);
                if (processor == null)
                    return
                        Json(
                            new
                            {
                                template = "failed"
                            });


                var retVal = await processor.ConvertFileToWebsitePreviewAsync(uri, index, cancellationToken);
                if (retVal.Content == null)
                {
                    return Json(new
                    {
                        template = "failed"
                        //preview = RenderRazorViewToString("_PreviewFailed",
                        //    Url.RouteUrl("ItemDownload2", new { boxId, itemId = id }))
                    });

                }
                if (!retVal.Content.Any())
                {
                    return Json("");
                }
                if (string.IsNullOrEmpty(retVal.ViewName))
                {
                    return Json(new { preview = retVal.Content.First() });
                }

                var res = new
                {
                    template = retVal.ViewName,
                    retVal.Content
                };
                return Json(res);

            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"GeneratePreview filename: {blobName}", ex);
                //ZboxWriteService.UpdatePreviewFailed(new PreviewFailedCommand(id));
                //if (index == 0)
                //{
                //    return Json(new
                //    {
                //        template = "failed"
                //        //preview = RenderRazorViewToString("_PreviewFailed",
                //        //    Url.RouteUrl("ItemDownload2", new { boxId, itemId = id }))
                //    });
                //}
                return Json("");
            }
        }
        #endregion
    }
}