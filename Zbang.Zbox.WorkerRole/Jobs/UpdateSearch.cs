using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class UpdateSearch : IJob
    {
        private const int NumberToReSyncWithoutWait = 20;
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
            while (m_KeepRunning)
            {
                ExecuteAsync().Wait();
            }
        }

        private async Task ExecuteAsync()
        {
            var quizUpdate = await UpdateQuiz();
            //var itemUpdate = await UpdateItem();
            var universityUpdate = await UpdateUniversity();
            var boxUpdate = await UpdateBox();
            if (/*itemUpdate || */ boxUpdate || universityUpdate || quizUpdate)
            {
                return;
            }
            await Task.Delay(TimeSpan.FromMinutes(1));
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
            }
            return updates.QuizzesToUpdate.Count() == NumberToReSyncWithoutWait
              || updates.QuizzesToDelete.Count() == NumberToReSyncWithoutWait;
        }

        private async Task<bool> UpdateItem()
        {
            var updates = await m_ZboxReadService.GetItemDirtyUpdatesAsync();
            if (updates.ItemsToUpdate.Any() || updates.ItemsToDelete.Any())
            {
                foreach (var elem in updates.ItemsToUpdate)
                {
                    elem.Content = await ExtractContentToUploadToSearch(elem);
                }
                var isSuccess =
                    await m_ItemSearchProvider.UpdateData(updates.ItemsToUpdate, updates.ItemsToDelete);
                var isSuccess2 =
                    await m_ItemSearchProvider2.UpdateData(updates.ItemsToUpdate, updates.ItemsToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.ItemsToDelete.Union(updates.ItemsToUpdate.Select(s => s.Id))));
                }
            }
            return updates.ItemsToUpdate.Count() == NumberToReSyncWithoutWait
              || updates.ItemsToDelete.Count() == NumberToReSyncWithoutWait;
        }

        private async Task<string> ExtractContentToUploadToSearch(ItemSearchDto elem)
        {
            try
            {
                var blob = m_BlobProvider.GetFile(elem.BlobName);
                var processor = m_FileProcessorFactory.GetProcessor(blob.Uri);
                if (processor == null) return null;
                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(TimeSpan.FromMinutes(2));
                var str = await processor.ExtractContent(blob.Uri, tokenSource.Token);
                if (string.IsNullOrEmpty(str))
                {
                    return str;
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
                return sb.ToString();
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
            }
            return updates.BoxesToUpdate.Count() == NumberToReSyncWithoutWait
               || updates.BoxesToDelete.Count() == NumberToReSyncWithoutWait;

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
            }
            return updates.UniversitiesToDelete.Count() == NumberToReSyncWithoutWait
                || updates.UniversitiesToUpdate.Count() == NumberToReSyncWithoutWait;
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
