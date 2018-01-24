using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Search;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchFlashcard : UpdateSearch, IJob
    {
        private readonly IFlashcardWriteSearchProvider _flashcardSearchProvider;
        private readonly IDocumentDbReadService _documentDbService;
        private readonly IZboxReadServiceWorkerRole _zboxReadService;
        private readonly IZboxWorkerRoleService _zboxWriteService;
        private readonly IContentWriteSearchProvider _contentSearchProvider;
        private readonly ILogger _logger;

        public UpdateSearchFlashcard(IFlashcardWriteSearchProvider flashcardSearchProvider, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService, IDocumentDbReadService documentDbService, IContentWriteSearchProvider contentSearchProvider,  ILogger logger)
        {
            _flashcardSearchProvider = flashcardSearchProvider;
            _zboxReadService = zboxReadService;
            _zboxWriteService = zboxWriteService;
            _documentDbService = documentDbService;
            _contentSearchProvider = contentSearchProvider;
            _logger = logger;
        }

        public string Name => nameof(UpdateSearchFlashcard);
        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int top = 100;
            var updates = await _zboxReadService.GetFlashcardsDirtyUpdatesAsync(instanceId, instanceCount, top, cancellationToken).ConfigureAwait(false);
            if (!updates.Updates.Any() && !updates.Deletes.Any()) return TimeToSleep.Increase;
            var toUpdates = updates.Updates.ToList();
            for (var i = toUpdates.Count - 1; i >= 0; i--)
            {
                var update = toUpdates[i];
                var flashcard = await _documentDbService.FlashcardAsync(update.Id).ConfigureAwait(false);
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

                //if (update.University != null && JaredUniversityIdPilot.Contains(update.University.Id))
                //{
                //    await JaredPilotAsync(update, cancellationToken).ConfigureAwait(false);
                //}
            }

            var isSuccess =
                await _flashcardSearchProvider.UpdateDataAsync(toUpdates, updates.Deletes.Select(s => s.Id), cancellationToken).ConfigureAwait(false);
            await _contentSearchProvider.UpdateDataAsync(null, updates.Deletes, cancellationToken).ConfigureAwait(false);
            if (isSuccess)
            {
                await _zboxWriteService.UpdateSearchFlashcardDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(
                        updates.Deletes.Select(s => s.Id).Union(updates.Updates.Select(s => s.Id)))).ConfigureAwait(false);
            }
            if (updates.Updates.Count() == top)
            {
                return TimeToSleep.Min;
            }
            return TimeToSleep.Same;
        }

        //private Task JaredPilotAsync(ItemSearchDto elem, CancellationToken token)
        //{
        //    return Task.CompletedTask;
        //    //if (elem.Language.GetValueOrDefault(Language.Undefined) == Language.Undefined)
        //    //{
        //    //    var result = await _watsonExtractProvider.GetLanguageAsync(elem.Content, token).ConfigureAwait(false);
        //    //    elem.Language = result;
        //    //    var commandLang = new AddLanguageToFlashcardCommand(elem.Id, result);
        //    //    _writeService.AddItemLanguage(commandLang);
        //    //}

        //    //if (elem.Language == Language.EnglishUs && elem.Tags.All(a=>a.Type != TagType.Watson))
        //    //{

        //    //    var result = await _watsonExtractProvider.GetConceptAsync(elem.Content, token).ConfigureAwait(false);
        //    //    if (result != null)
        //    //    {
        //    //        var tags = result as IList<string> ?? result.ToList();
        //    //        elem.Tags.AddRange(tags.Select(s => new ItemSearchTag { Name = s }));
        //    //        var z = new AssignTagsToFlashcardCommand(elem.Id, tags, TagType.Watson);
        //    //        await _writeService.AddItemTagAsync(z).ConfigureAwait(false);
        //    //    }
        //    //}

        //    //await m_ContentSearchProvider.UpdateDataAsync(elem, null, token).ConfigureAwait(false);
        //}

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            _logger.Warning("item index " + index + " count " + count);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await DoProcessAsync(cancellationToken, index, count).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex);
                }
            }
            _logger.Error($"{Name} On finish run");
        }
    }
}
