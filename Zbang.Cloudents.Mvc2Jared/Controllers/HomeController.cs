using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries.Jared;
using System.Threading;
using Zbang.Zbox.ReadServices;
using System.Linq;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Cloudents.Mvc2Jared.Models;
using Zbang.Zbox.Infrastructure.Enums;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Consts;

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
        [HttpGet, ActionName("University")]
        public async Task<JsonResult> UniAsync(string term, CancellationToken cancellationToken)
        {
            var retVal = await m_readService.GetUniAsync(new SearchTermQuery(term));
            return Json(retVal,JsonRequestBehavior.AllowGet);
        }
        [HttpGet, ActionName("Department")]
        public async Task<JsonResult> DepartmentAsync(string term, CancellationToken cancellationToken)
        {
            var retVal = await m_readService.GetDepartmentAsync(new SearchTermQuery(term));
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }
        [HttpGet, ActionName("Tag")]
        public async Task<JsonResult> TabAsync(string term, CancellationToken cancellationToken)
        {
            var retVal = await m_readService.GetTagAsync(new SearchTermQuery(term));
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }
        [HttpPost,ActionName("Save")]
        public async Task<JsonResult> SaveAsync(SaveItemTags model)
        {
            bool rename = (!String.IsNullOrEmpty(model.ItemName));
            bool tabChanged = (model.TabId.HasValue);
            bool isAddTags = ( model.NewTags != null&& model.NewTags.Any());
            bool isRemoveTags = ( model.RemoveTags != null&&model.RemoveTags.Any());
            if (rename) Rename(model.ItemId, model.ItemName);
            if (tabChanged)  await AddItemToTabAsync(model.ItemId, model.TabId, model.BoxId);
            if (isAddTags)addTagsToDoc(model.ItemId, model.NewTags);
            if (isRemoveTags) {
                var command = new RemoveTagsFromDocumentCommand(model.ItemId, model.RemoveTags);
                m_writeService.RemoveItemTag(command);
            }
            var save = new SetReviewedDocumentCommand(model.ItemId);
            m_writeService.SetReviewed(save);
            return Json("");
        }
        private void addTagsToDoc(long itemId,IEnumerable<string> newTags)
        {
            var z = new AssignTagsToDocumentCommand(itemId, newTags, TagType.Backoffice);
            m_writeService.AddItemTag(z);
        }
        private async Task<JsonResult> AddItemToTabAsync(long itemId, Guid? tabId,long boxId)
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
        #region Preview
        [HttpGet, ActionName("Preview")]
        [AsyncTimeout(TimeConst.Minute * 3 * 1000)]
        public async Task<JsonResult> PreviewAsync(string blobName, long id,CancellationToken cancellationToken)
        {
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
                            }, JsonRequestBehavior.AllowGet);


                var retVal = await processor.ConvertFileToWebsitePreviewAsync(uri, 0, cancellationToken);
                if (retVal.Content == null)
                {
                    return Json(new
                    {
                        template = "failed"
                    }, JsonRequestBehavior.AllowGet);

                }
                if (!retVal.Content.Any())
                {
                    return Json("no Data", JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(retVal.ViewName))
                {
                    return Json(new { preview = retVal.Content.First() }, JsonRequestBehavior.AllowGet);
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
                return Json("exception occuried", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        [HttpPost, ActionName("Delete")]
        public async Task<JsonResult> DeleteAsync(long itemId)
        {
            try
            {
                var command = new DeleteItemCommand(itemId, 1);
                await m_writeService.DeleteItemAsync(command);
                return Json(itemId);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"DeleteItem user: 1  itemId {itemId}", ex);
                return Json(ex);
            }
        }
    }
}