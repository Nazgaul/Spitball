using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries.Jared;
using System.Threading;
using Zbang.Zbox.ReadServices;
using System.Linq;
using Zbang.Zbox.Infrastructure.Storage;
using System.Collections.Generic;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc2Jared.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : Controller
    {
        private readonly IZboxCacheReadService m_readService;
        private readonly IZboxWriteService m_writeService;
        private readonly Lazy<IFileProcessorFactory> m_FileProcessorFactory;
        private readonly Lazy<IBlobProvider2<FilesContainerName>> m_BlobProviderFiles;
        public HomeController(IZboxCacheReadService readService,
            IZboxWriteService writeService,
            Lazy<IBlobProvider2<FilesContainerName>> blobProviderFiles,
            Lazy<IFileProcessorFactory> fileProcessorFactory)
        {
            m_readService = readService;
            m_writeService = writeService;
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
        [HttpPost,ActionName("Save")]
        public async Task<JsonResult> SaveAsync(long itemId,long boxId,string name, Guid? tabId, IEnumerable<string> newTags, IEnumerable<string> removeTags)
        {
            if (name.Length>0) Rename(itemId, name);
            if (tabId.HasValue) { var ab = await AddItemToTabAsync(itemId, tabId, boxId); }
            return Json("");
        }
        public async Task<JsonResult> AddItemToTabAsync(long itemId, Guid? tabId,long boxId)
        {
            var command = new AssignItemToTabCommand(itemId, tabId, boxId, 1);
            await m_writeService.AssignBoxItemToTabAsync(command);
            return Json("");
        }
        private JsonResult Rename(long id, string name)
        {
            try
            {
                var command = new ChangeFileNameCommand(id, name,1);
                var result = m_writeService.ChangeFileName(command);
                return Json(new
                {
                    name = result.Name,
                    url = result.Url
                });
            }

            catch (UnauthorizedAccessException)
            {
                return Json("You need to follow this box in order to change file name");
            }
            catch (ArgumentException ex)
            {
                return Json(ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"ChangeFileName newFileName {name} ItemUid {id} userId 1", ex);
                return Json("Error");
            }


        }
        [HttpGet,ActionName("Tabs")]
        public async Task<JsonResult> TabsAsync(long id)
        {
            try
            {
                var query = new GetBoxQuery(id);
                var result = await m_readService.GetBoxTabsAsync(query);
                return Json(result,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"Box Tabs id {id}", ex);
                return Json("error");
            }
        }
        #region Preview
        [HttpGet, ActionName("Preview")]
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
                return Json(new
                {
                    template = retVal.ViewName,
                    retVal.Content
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"GeneratePreview filename: {blobName}", ex);
                return Json("");
            }
        }
        #endregion
    }
}