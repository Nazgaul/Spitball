using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchFlashcard : UpdateSearch, IJob
    {
        private readonly IFlashcardWriteSearchProvider m_FlashcardSearchProvider;
        private readonly IDocumentDbReadService m_DocumentDbService;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly IContentWriteSearchProvider m_ContentSearchProvider;

        public UpdateSearchFlashcard(IFlashcardWriteSearchProvider flashcardSearchProvider, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService, IDocumentDbReadService documentDbService, IContentWriteSearchProvider contentSearchProvider)
        {
            m_FlashcardSearchProvider = flashcardSearchProvider;
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_DocumentDbService = documentDbService;
            m_ContentSearchProvider = contentSearchProvider;
        }

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int top = 100;
            var updates = await m_ZboxReadService.GetFlashcardsDirtyUpdatesAsync(instanceId, instanceCount, top, cancellationToken);
            if (!updates.Updates.Any() && !updates.Deletes.Any()) return TimeToSleep.Increase;
            //TraceLog.WriteInfo(GetPrefix(),
            //    $"updating {updates.Updates.Count()} deleting {updates.Deletes.Count()}");
            var toUpdates = updates.Updates.ToList();
            for (int i = toUpdates.Count - 1; i >= 0; i--)
            {
                var update = toUpdates[i];
                var flashcard = await m_DocumentDbService.FlashcardAsync(update.Id);
                if (flashcard == null)
                {
                    toUpdates.RemoveAt(i);
                    continue;
                }
                if (flashcard.Cards == null)
                {
                    toUpdates.RemoveAt(i);
                    continue;
                }
                update.BackCards = flashcard.Cards.Select(s => s.Cover).Where(w => !string.IsNullOrEmpty(w.Text)).Select(s => s.Text);
                update.FrontCards = flashcard.Cards.Select(s => s.Front).Where(w => !string.IsNullOrEmpty(w.Text)).Select(s => s.Text);
                if (update.UniversityId == JaredUniversityIdPilot)
                {

                    await JaredPilotAsync(update, cancellationToken);
                }
            }

            var isSuccess =
                await m_FlashcardSearchProvider.UpdateDataAsync(toUpdates, updates.Deletes.Select(s => s.Id), cancellationToken);
            await m_ContentSearchProvider.UpdateDataAsync(null, updates.Deletes, cancellationToken);
            if (isSuccess)
            {
                await m_ZboxWriteService.UpdateSearchFlashcardDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(
                        updates.Deletes.Select(s => s.Id).Union(updates.Updates.Select(s => s.Id))));
            }
            if (updates.Updates.Count() == top)
            {
                return TimeToSleep.Min;
            }
            return TimeToSleep.Same;
        }

        private async Task JaredPilotAsync(FlashcardSearchDto elem, CancellationToken token)
        {
            await m_ContentSearchProvider.UpdateDataAsync(elem, null, token);
        }

        protected override string GetPrefix()
        {
            return "Search Flashcard";
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("flashcard index " + index + " count " + count);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await DoProcessAsync(cancellationToken, index, count);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
            }
            TraceLog.WriteError("On finish run");
        }
    }
}
