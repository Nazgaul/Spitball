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
    public class UpdateSearchQuiz : UpdateSearch, IJob
    {
        private readonly IQuizWriteSearchProvider2 m_QuizSearchProvider;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly IContentWriteSearchProvider m_ContentSearchProvider;
        private readonly IZboxWriteService m_WriteService;
        private readonly IWatsonExtract m_WatsonExtractProvider;
        private readonly IDetectLanguage m_LanguageDetect;

        private const string PrefixLog = "Search Quiz";
        public UpdateSearchQuiz(IQuizWriteSearchProvider2 quizSearchProvider, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService, IContentWriteSearchProvider contentSearchProvider, IZboxWriteService writeService, IWatsonExtract watsonExtractProvider, IDetectLanguage languageDetect)
        {
            m_QuizSearchProvider = quizSearchProvider;
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_ContentSearchProvider = contentSearchProvider;
            m_WriteService = writeService;
            m_WatsonExtractProvider = watsonExtractProvider;
            m_LanguageDetect = languageDetect;
        }
        

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("quiz index " + index + " count " + count);
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

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int top = 100;
            var updates = await m_ZboxReadService.GetQuizzesDirtyUpdatesAsync(instanceId, instanceCount, top);
            if (!updates.QuizzesToUpdate.Any() && !updates.QuizzesToDelete.Any()) return TimeToSleep.Increase;

            //foreach (var quiz in updates.QuizzesToUpdate.Where(w=>w.UniversityId == JaredUniversityIdPilot))
            //{
            //    await JaredPilotAsync(quiz, cancellationToken);
            //}

            var isSuccess =
                await m_QuizSearchProvider.UpdateDataAsync(updates.QuizzesToUpdate, updates.QuizzesToDelete.Select(s=>s.Id));
            await m_ContentSearchProvider.UpdateDataAsync(null, updates.QuizzesToDelete, cancellationToken);
            if (isSuccess)
            {
                await m_ZboxWriteService.UpdateSearchQuizDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(
                        updates.QuizzesToDelete.Select(s => s.Id).Union(updates.QuizzesToUpdate.Select(s => s.Id))));
            }
            if (updates.QuizzesToUpdate.Count() == top)
            {
                return TimeToSleep.Min;
            }
            return TimeToSleep.Same;
        }

        private async Task JaredPilotAsync(QuizSearchDto elem, CancellationToken token)
        {
            if (!elem.Language.HasValue)
            {
                elem.Language = m_LanguageDetect.DoWork(elem.Content);
                var commandLang = new AddLanguageToQuizCommand(elem.Id, elem.Language.Value);
                m_WriteService.AddItemLanguage(commandLang);
            }

            if (elem.Language == Infrastructure.Culture.Language.EnglishUs/* && (elem.Tags == null || !elem.Tags.Any())*/)
            {

                var result = (await m_WatsonExtractProvider.GetConceptAsync(elem.Content, token)).ToList();
                elem.Tags.AddRange(result.Select(s => new ItemSearchTag { Name = s }));
                var z = new AssignTagsToQuizCommand(elem.Id, result, TagType.Watson);
                m_WriteService.AddItemTag(z);
            }

            var command = new UpdateQuizCourseTagCommand(elem.Id, elem.BoxName, elem.BoxCode, elem.BoxProfessor);
            m_WriteService.UpdateItemCourseTag(command);

            await m_ContentSearchProvider.UpdateDataAsync(elem, null, token);
        }

        protected override string GetPrefix()
        {
            return PrefixLog;
        }
    }
}
