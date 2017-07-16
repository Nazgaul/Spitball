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
    public class UpdateSearchQuiz : UpdateSearch, IJob
    {
        private readonly IQuizWriteSearchProvider2 m_QuizSearchProvider;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly IContentWriteSearchProvider m_ContentSearchProvider;
        private readonly IZboxWriteService m_WriteService;
        private readonly IWatsonExtract m_WatsonExtractProvider;

        public UpdateSearchQuiz(IQuizWriteSearchProvider2 quizSearchProvider, IZboxReadServiceWorkerRole zboxReadService,
            IZboxWorkerRoleService zboxWriteService, IContentWriteSearchProvider contentSearchProvider,
            IZboxWriteService writeService, IWatsonExtract watsonExtractProvider)
        {
            m_QuizSearchProvider = quizSearchProvider;
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_ContentSearchProvider = contentSearchProvider;
            m_WriteService = writeService;
            m_WatsonExtractProvider = watsonExtractProvider;
        }

        public string Name => nameof(UpdateSearchQuiz);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("quiz index " + index + " count " + count);
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

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            try
            {
                const int top = 100;
                var updates =
                    await m_ZboxReadService.GetQuizzesDirtyUpdatesAsync(instanceId, instanceCount, top)
                        .ConfigureAwait(false);
                if (!updates.QuizzesToUpdate.Any() && !updates.QuizzesToDelete.Any()) return TimeToSleep.Increase;
                TraceLog.WriteInfo($"{Name} is doing process");
                foreach (var quiz in updates.QuizzesToUpdate.Where( w => w.University !=null && JaredUniversityIdPilot.Contains(w.University.Id)))
                {
                    await JaredPilotAsync(quiz, cancellationToken).ConfigureAwait(false);
                }

                var isSuccess =
                    await m_QuizSearchProvider.UpdateDataAsync(updates.QuizzesToUpdate,
                        updates.QuizzesToDelete.Select(s => s.Id)).ConfigureAwait(false);
                await m_ContentSearchProvider.UpdateDataAsync(null, updates.QuizzesToDelete, cancellationToken)
                    .ConfigureAwait(false);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchQuizDirtyToRegularAsync(
                            new UpdateDirtyToRegularCommand(
                                updates.QuizzesToDelete.Select(s => s.Id)
                                    .Union(updates.QuizzesToUpdate.Select(s => s.Id))))
                        .ConfigureAwait(false);
                }
                if (updates.QuizzesToUpdate.Count() == top)
                {
                    return TimeToSleep.Min;
                }
                return TimeToSleep.Same;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(Name, ex);
                return TimeToSleep.Increase;
            }
        }

        private async Task JaredPilotAsync(QuizSearchDto elem, CancellationToken token)
        {
            if (elem.Language.GetValueOrDefault(Language.Undefined) == Language.Undefined)
            {
                var result = await m_WatsonExtractProvider.GetLanguageAsync(elem.Content,token).ConfigureAwait(false);
                elem.Language = result;
                var commandLang = new AddLanguageToQuizCommand(elem.Id, result);
                m_WriteService.AddItemLanguage(commandLang);
            }

            if (elem.Language == Language.EnglishUs && elem.Tags.All(a => a.Type != TagType.Watson))
            {

                var result = await m_WatsonExtractProvider.GetConceptAsync(elem.Content, token).ConfigureAwait(false);
                if (result != null)
                {
                    var tags = result as IList<string> ?? result.ToList();
                    elem.Tags.AddRange(tags.Select(s => new ItemSearchTag { Name = s }));
                    var z = new AssignTagsToQuizCommand(elem.Id, tags, TagType.Watson);
                    await m_WriteService.AddItemTagAsync(z).ConfigureAwait(false);
                }
            }

            await m_ContentSearchProvider.UpdateDataAsync(elem, null, token).ConfigureAwait(false);
        }

        //protected override string GetPrefix()
        //{
        //    return PrefixLog;
        //}
    }
}
