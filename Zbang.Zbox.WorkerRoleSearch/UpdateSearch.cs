using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearch : IJob
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IUniversityWriteSearchProvider2 m_UniversitySearchProvider;
        private readonly IBoxWriteSearchProvider m_BoxSearchProvider;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IQuizWriteSearchProvider m_QuizSearchProvider;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IItemWriteSearchProvider2 m_ItemSearchProvider2;
        private readonly ICloudBlockProvider m_BlobProvider;

        private const string PrefixLog = "Search";

        public UpdateSearch(IZboxReadServiceWorkerRole zboxReadService,
            IUniversityWriteSearchProvider2 zboxWriteSearchProvider,
            IZboxWriteService zboxWriteService,
            IBoxWriteSearchProvider boxSearchProvider,
            IQuizWriteSearchProvider quizSearchProvider,
            IFileProcessorFactory fileProcessorFactory,
            IItemWriteSearchProvider2 itemSearchProvider2, ICloudBlockProvider blobProvider)
        {
            m_ZboxReadService = zboxReadService;
            m_UniversitySearchProvider = zboxWriteSearchProvider;
            m_ZboxWriteService = zboxWriteService;
            m_BoxSearchProvider = boxSearchProvider;
            m_QuizSearchProvider = quizSearchProvider;
            m_FileProcessorFactory = fileProcessorFactory;
            m_ItemSearchProvider2 = itemSearchProvider2;
            m_BlobProvider = blobProvider;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            TraceLog.WriteInfo(PrefixLog, "run job search update ");
            var index = GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteInfo(PrefixLog, "index: " + index + " count " + count);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    TraceLog.WriteInfo(PrefixLog, "starting update search cycle");
                    //var tQuizUpdate = UpdateQuiz();
                    var tItemUpdate = UpdateItem(index, count);
                    //var tUniversityUpdate = UpdateUniversity();
                    //var tBoxUpdate = UpdateBox();
                    await Task.WhenAll(
                       // tQuizUpdate,
                        tItemUpdate
                       // tUniversityUpdate,
                       // tBoxUpdate
                        );

                    if (tItemUpdate.Result
                      //  || tBoxUpdate.Result
                       // || tUniversityUpdate.Result
                       // || tQuizUpdate.Result
                        )
                    {
                        TraceLog.WriteInfo(PrefixLog, "finish update search cycle invoking new cycle");

                    }
                    else
                    {
                        TraceLog.WriteInfo(PrefixLog, "finish update search cycle going to sleep");
                        await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
            }
            TraceLog.WriteError("On finish run");
        }

        private int GetIndex()
        {
            int currentIndex;

            string instanceId = RoleEnvironment.CurrentRoleInstance.Id;
            bool withSuccess = int.TryParse(instanceId.Substring(instanceId.LastIndexOf(".", StringComparison.Ordinal) + 1), out currentIndex);
            if (!withSuccess)
            {
                int.TryParse(instanceId.Substring(instanceId.LastIndexOf("_", StringComparison.Ordinal) + 1), out currentIndex);
            }
            return currentIndex;
        }



        private async Task<bool> UpdateQuiz()
        {
            TraceLog.WriteInfo(PrefixLog, "working on quiz");
            var updates = await m_ZboxReadService.GetQuizzesDirtyUpdatesAsync();
            if (updates.QuizzesToUpdate.Any() || updates.QuizzesToDelete.Any())
            {
                TraceLog.WriteInfo(PrefixLog, string.Format("quiz updating {0} deleting {1}", updates.QuizzesToUpdate.Count(),
                    updates.QuizzesToDelete.Count()));
                var isSuccess =
                    await m_QuizSearchProvider.UpdateData(updates.QuizzesToUpdate, updates.QuizzesToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchQuizDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.QuizzesToDelete.Union(updates.QuizzesToUpdate.Select(s => s.Id))));
                }
                return true;
            }
            TraceLog.WriteInfo(PrefixLog, "nothing on quiz");
            return false;
        }

        private async Task<bool> UpdateItem(int instanceId, int instanceCount)
        {
            TraceLog.WriteInfo(PrefixLog, "working on item");
            //var updates = new ItemToUpdateSearchDto
            //{
            //    ItemsToUpdate = new List<ItemSearchDto>
            //    {
            //        new ItemSearchDto {Id = 291153, BlobName = "12e5fe33-aca7-4aee-b194-2c99a0739d04.docx"}
            //    },
            //    ItemsToDelete = new List<long>()

            //};
            var updates = await m_ZboxReadService.GetItemDirtyUpdatesAsync(instanceId, instanceCount);
            if (updates.ItemsToUpdate.Any() || updates.ItemsToDelete.Any())
            {
                TraceLog.WriteInfo(PrefixLog, string.Format("item updating {0} deleting {1}", updates.ItemsToUpdate.Count(),
                    updates.ItemsToDelete.Count()));
                foreach (var elem in updates.ItemsToUpdate)
                {
                    elem.Content = ExtractContentToUploadToSearch(elem);
                }
                //await m_ItemSearchProvider.UpdateData(updates.ItemsToUpdate, updates.ItemsToDelete);
                var isSuccess2 =
                    await m_ItemSearchProvider2.UpdateData(updates.ItemsToUpdate, updates.ItemsToDelete);
                if (isSuccess2)
                {
                    await m_ZboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.ItemsToDelete.Union(updates.ItemsToUpdate.Select(s => s.Id))));
                }
                return true;
            }
            TraceLog.WriteInfo(PrefixLog, "nothing on item");
            return false;
        }

        readonly TimeSpan m_TimeToWait = TimeSpan.FromMinutes(3);
        private string ExtractContentToUploadToSearch(ItemSearchDto elem)
        {
            TraceLog.WriteInfo(PrefixLog, "search processing " + elem);
            try
            {
                var wait = new ManualResetEvent(false);

                var blob = m_BlobProvider.GetFile(elem.BlobName);
                var processor = m_FileProcessorFactory.GetProcessor(blob.Uri);
                if (processor == null) return null;
                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(m_TimeToWait);

                string str = null;

                var work = new Thread(async () =>
                {
                    try
                    {

                        str = await processor.ExtractContent(blob.Uri, tokenSource.Token);
                        TraceLog.WriteInfo(PrefixLog, "finish extract document content on elem" + elem);
                        if (string.IsNullOrEmpty(str))
                        {
                            wait.Set();
                            return;
                        }
                        var sb = new StringBuilder();
                        int byteCount = 0;
                        var buffer = new char[1];
                        foreach (var ch in str)
                        {
                            if (char.IsSurrogate(ch))
                            {
                                continue;
                            }
                            buffer[0] = ch;
                            byteCount += Encoding.UTF8.GetByteCount(buffer);
                            if (byteCount > 15000000)
                            {
                                Trace.TraceWarning("reach limit");
                                // Couldn't add this character. Return its index
                                break;
                            }
                            sb.Append(ch);
                        }
                        str = sb.ToString();
                        wait.Set();
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("on elem " + elem, ex);
                    }
                });
                work.Start();
                Boolean signal = wait.WaitOne(m_TimeToWait);
                if (!signal)
                {
                    work.Abort();
                    TraceLog.WriteError("aborting returning null on elem " + elem);
                    return null;
                }
                TraceLog.WriteInfo(PrefixLog, "returning result" + elem);
                return str;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on elem " + elem, ex);
                return null;
            }
        }

        private async Task<bool> UpdateBox()
        {
            TraceLog.WriteInfo(PrefixLog, "working on box");
            var updates = await m_ZboxReadService.GetBoxDirtyUpdates();
            if (updates.BoxesToUpdate.Any() || updates.BoxesToDelete.Any())
            {
                TraceLog.WriteInfo(PrefixLog, string.Format("box updating {0} deleting {1}", updates.BoxesToUpdate.Count(),
                    updates.BoxesToDelete.Count()));
                var isSuccess =
                    await m_BoxSearchProvider.UpdateData(updates.BoxesToUpdate, updates.BoxesToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchBoxDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.BoxesToDelete.Union(updates.BoxesToUpdate.Select(s => s.Id))));
                }

                return true;
            }
            TraceLog.WriteInfo(PrefixLog, "nothing on box");
            return false;
        }



        private async Task<bool> UpdateUniversity()
        {
            TraceLog.WriteInfo(PrefixLog, "working on university");
            var updates = await m_ZboxReadService.GetUniversityDirtyUpdates();
            if (updates.UniversitiesToDelete.Any() || updates.UniversitiesToUpdate.Any())
            {
                TraceLog.WriteInfo(PrefixLog, string.Format("university updating {0} deleting {1}", updates.UniversitiesToUpdate.Count(),
                    updates.UniversitiesToDelete.Count()));
                var isSuccess =
                    await m_UniversitySearchProvider.UpdateData(updates.UniversitiesToUpdate, updates.UniversitiesToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchUniversityDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.UniversitiesToDelete.Union(updates.UniversitiesToUpdate.Select(s => s.Id))));
                }
                return true;
            }
            TraceLog.WriteInfo(PrefixLog, "nothing on university");
            return false;
        }


    }
}
