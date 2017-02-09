using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Enums;
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
        private readonly IZboxWriteService m_WriteService;
        private readonly IWatsonExtract m_WatsonExtractProvider;
        private readonly IDetectLanguage m_LanguageDetect;

        public UpdateSearchFlashcard(IFlashcardWriteSearchProvider flashcardSearchProvider, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService, IDocumentDbReadService documentDbService, IContentWriteSearchProvider contentSearchProvider, IZboxWriteService writeService, IWatsonExtract watsonExtractProvider, IDetectLanguage languageDetect)
        {
            m_FlashcardSearchProvider = flashcardSearchProvider;
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_DocumentDbService = documentDbService;
            m_ContentSearchProvider = contentSearchProvider;
            m_WriteService = writeService;
            m_WatsonExtractProvider = watsonExtractProvider;
            m_LanguageDetect = languageDetect;
        }

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int top = 100;
            var updates = await m_ZboxReadService.GetFlashcardsDirtyUpdatesAsync(instanceId, instanceCount, top, cancellationToken);
            if (!updates.Updates.Any() && !updates.Deletes.Any()) return TimeToSleep.Increase;
            var toUpdates = updates.Updates.ToList();
            for (var i = toUpdates.Count - 1; i >= 0; i--)
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

        private async Task JaredPilotAsync(ItemSearchDto elem, CancellationToken token)
        {
            if (!elem.Language.HasValue)
            {
                elem.Language = m_LanguageDetect.DoWork(elem.Content);
                var commandLang = new AddLanguageToFlashcardCommand(elem.Id, elem.Language.Value);
                m_WriteService.AddItemLanguage(commandLang);
            }

            if (elem.Language == Infrastructure.Culture.Language.EnglishUs && elem.Tags.All(a=>a.Type != TagType.Watson))
            {

                var result = (await m_WatsonExtractProvider.GetConceptAsync(elem.Content, token)).ToList();
                elem.Tags.AddRange(result.Select(s => new ItemSearchTag { Name = s }));
                var z = new AssignTagsToFlashcardCommand(elem.Id, result, TagType.Watson);
                m_WriteService.AddItemTag(z);
            }

            var command = new UpdateFlashcardCourseTagCommand(elem.Id, elem.BoxName, elem.BoxCode, elem.BoxProfessor);
            m_WriteService.UpdateItemCourseTag(command);

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
