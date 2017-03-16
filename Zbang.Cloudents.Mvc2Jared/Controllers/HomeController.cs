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
        [HttpPost,ActionName("Save")]
        public async Task<JsonResult> SaveAsync(long itemId,string name,string docType, IEnumerable<string> newTags, IEnumerable<string> removeTags)
        {
            int a;
            a = 5;
            return Json("");
        }
        //public JsonResult Rename(long id,string name)
        //{
        //    try
        //    {
        //        var command = new ChangeFileNameCommand(id,name);
        //        var result = ZboxWriteService.ChangeFileName(command);
        //        return JsonOk(new
        //        {
        //            name = result.Name,
        //            url = result.Url
        //        });
        //    }

        //    catch (UnauthorizedAccessException)
        //    {
        //        return JsonError("You need to follow this box in order to change file name");
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return JsonError(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError($"ChangeFileName newFileName {model.NewName} ItemUid {model.Id} userId {userId}", ex);
        //        return JsonError("Error");
        //    }


        //}
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