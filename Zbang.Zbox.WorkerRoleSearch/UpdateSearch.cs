using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearch : IJob
    {
        private bool m_KeepRunning;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IUniversityWriteSearchProvider2 m_UniversitySearchProvider;
        private readonly IBoxWriteSearchProvider m_BoxSearchProvider;
        private readonly IItemWriteSearchProvider m_ItemSearchProvider;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IQuizWriteSearchProvider m_QuizSearchProvider;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IItemWriteSearchProvider2 m_ItemSearchProvider2;
        private readonly ICloudBlockProvider m_BlobProvider;

        public UpdateSearch(IZboxReadServiceWorkerRole zboxReadService,
            IUniversityWriteSearchProvider2 zboxWriteSearchProvider,
            IZboxWriteService zboxWriteService,
            IBoxWriteSearchProvider boxSearchProvider,
            IItemWriteSearchProvider itemSearchProvider,
            IQuizWriteSearchProvider quizSearchProvider,
            IFileProcessorFactory fileProcessorFactory,
            IItemWriteSearchProvider2 itemSearchProvider2, ICloudBlockProvider blobProvider)
        {
            m_ZboxReadService = zboxReadService;
            m_UniversitySearchProvider = zboxWriteSearchProvider;
            m_ZboxWriteService = zboxWriteService;
            m_BoxSearchProvider = boxSearchProvider;
            m_ItemSearchProvider = itemSearchProvider;
            m_QuizSearchProvider = quizSearchProvider;
            m_FileProcessorFactory = fileProcessorFactory;
            m_ItemSearchProvider2 = itemSearchProvider2;
            m_BlobProvider = blobProvider;
        }

        public void Run()
        {
            m_KeepRunning = true;
            var index = GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteInfo("index: " + index + " count " + count);
            while (m_KeepRunning)
            {
                ExecuteAsync(index, count).Wait();
            }
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

        private async Task ExecuteAsync(int index, int count)
        {
            try
            {
               
                var quizUpdate = await UpdateQuiz();
                var itemUpdate = await UpdateItem(index, count);
                var universityUpdate = await UpdateUniversity();
                var boxUpdate = await UpdateBox();
                if (itemUpdate || boxUpdate || universityUpdate || quizUpdate)
                {
                    return;
                }
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
            }
        }

        private async Task<bool> UpdateQuiz()
        {
            var updates = await m_ZboxReadService.GetQuizzesDirtyUpdatesAsync();
            if (updates.QuizzesToUpdate.Any() || updates.QuizzesToDelete.Any())
            {
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
            return false;
        }

        private async Task<bool> UpdateItem(int instanceId, int instanceCount)
        {
            var updates = await m_ZboxReadService.GetItemDirtyUpdatesAsync(instanceId, instanceCount);
            if (updates.ItemsToUpdate.Any() || updates.ItemsToDelete.Any())
            {

                foreach (var elem in updates.ItemsToUpdate)
                {
                    elem.Content = ExtractContentToUploadToSearch(elem);
                }
                await m_ItemSearchProvider.UpdateData(updates.ItemsToUpdate, updates.ItemsToDelete);
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
            TraceLog.WriteInfo("nothing to work on");
            return false;
        }

        readonly TimeSpan m_TimeToWait = TimeSpan.FromMinutes(3);
        private string ExtractContentToUploadToSearch(ItemSearchDto elem)
        {
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
                        if (string.IsNullOrEmpty(str))
                        {
                            wait.Set();
                            return;
                        }
                        var sb = new StringBuilder();
                        foreach (var ch in str)
                        {
                            if (char.IsSurrogate(ch))
                            {
                                continue;
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
                    TraceLog.WriteError("aborting on elem" + elem);
                    return null;
                }


                TraceLog.WriteInfo("processing " + elem.Id);

                return str.RemoveEndOfString(15000000);


            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on elem " + elem, ex);
                return null;
            }
        }

        private async Task<bool> UpdateBox()
        {
            var updates = await m_ZboxReadService.GetBoxDirtyUpdates();
            if (updates.BoxesToUpdate.Any() || updates.BoxesToDelete.Any())
            {
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
            return false;

        }



        private async Task<bool> UpdateUniversity()
        {
            var updates = await m_ZboxReadService.GetUniversityDirtyUpdates();
            if (updates.UniversitiesToDelete.Any() || updates.UniversitiesToUpdate.Any())
            {
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
            return false;
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
