using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Culture;
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

        public UpdateSearchFlashcard(IFlashcardWriteSearchProvider flashcardSearchProvider, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService, IDocumentDbReadService documentDbService, IContentWriteSearchProvider contentSearchProvider, IZboxWriteService writeService, IWatsonExtract watsonExtractProvider)
        {
            m_FlashcardSearchProvider = flashcardSearchProvider;
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_DocumentDbService = documentDbService;
            m_ContentSearchProvider = contentSearchProvider;
            m_WriteService = writeService;
            m_WatsonExtractProvider = watsonExtractProvider;
        }

        public string Name => nameof(UpdateSearchFlashcard);
        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int top = 100;
            var updates = await m_ZboxReadService.GetFlashcardsDirtyUpdatesAsync(instanceId, instanceCount, top, cancellationToken).ConfigureAwait(false);
            if (!updates.Updates.Any() && !updates.Deletes.Any()) return TimeToSleep.Increase;
            TraceLog.WriteInfo($"{Name} is doing process");
            var toUpdates = updates.Updates.ToList();
            for (var i = toUpdates.Count - 1; i >= 0; i--)
            {
                var update = toUpdates[i];
                var flashcard = await m_DocumentDbService.FlashcardAsync(update.Id).ConfigureAwait(false);
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
                if (!flashcard.Cards.Any())
                {
                    toUpdates.RemoveAt(i);
                    continue;
                }
                update.BackCards = flashcard.Cards.Select(s => s.Cover).Where(w => !string.IsNullOrEmpty(w.Text)).Select(s => s.Text);
                update.FrontCards = flashcard.Cards.Select(s => s.Front).Where(w => !string.IsNullOrEmpty(w.Text)).Select(s => s.Text);
                var firstCard = flashcard.Cards.First();
                update.FrontText = firstCard.Front.Text;
                update.FrontImage = firstCard.Front.Image;
                update.CoverText = firstCard.Cover.Text;
                update.CoverImage = firstCard.Cover.Image;

                if (update.University != null && JaredUniversityIdPilot.Contains(update.University.Id))
                {
                    await JaredPilotAsync(update, cancellationToken).ConfigureAwait(false);
                }
            }

            var isSuccess =
                await m_FlashcardSearchProvider.UpdateDataAsync(toUpdates, updates.Deletes.Select(s => s.Id), cancellationToken).ConfigureAwait(false);
            await m_ContentSearchProvider.UpdateDataAsync(null, updates.Deletes, cancellationToken).ConfigureAwait(false);
            if (isSuccess)
            {
                await m_ZboxWriteService.UpdateSearchFlashcardDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(
                        updates.Deletes.Select(s => s.Id).Union(updates.Updates.Select(s => s.Id)))).ConfigureAwait(false);
            }
            if (updates.Updates.Count() == top)
            {
                return TimeToSleep.Min;
            }
            return TimeToSleep.Same;
        }

        private async Task JaredPilotAsync(ItemSearchDto elem, CancellationToken token)
        {
            if (elem.Language.GetValueOrDefault(Language.Undefined) == Language.Undefined)
            {
                var result = await m_WatsonExtractProvider.GetLanguageAsync(elem.Content, token).ConfigureAwait(false);
                elem.Language = result;
                var commandLang = new AddLanguageToFlashcardCommand(elem.Id, result);
                m_WriteService.AddItemLanguage(commandLang);
            }

            if (elem.Language == Language.EnglishUs && elem.Tags.All(a=>a.Type != TagType.Watson))
            {

                var result = await m_WatsonExtractProvider.GetConceptAsync(elem.Content, token).ConfigureAwait(false);
                if (result != null)
                {
                    var tags = result as IList<string> ?? result.ToList();
                    elem.Tags.AddRange(tags.Select(s => new ItemSearchTag { Name = s }));
                    var z = new AssignTagsToFlashcardCommand(elem.Id, tags, TagType.Watson);
                    await m_WriteService.AddItemTagAsync(z).ConfigureAwait(false);
                }
            }

            //var command = new UpdateFlashcardCourseTagCommand(elem.Id, elem.BoxName, elem.BoxCode, elem.BoxProfessor);
            //m_WriteService.UpdateItemCourseTag(command);

            await m_ContentSearchProvider.UpdateDataAsync(elem, null, token).ConfigureAwait(false);
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
                    await DoProcessAsync(cancellationToken, index, count).ConfigureAwait(false);
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
