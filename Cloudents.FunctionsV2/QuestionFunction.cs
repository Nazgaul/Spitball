using Cloudents.FunctionsV2.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class QuestionFunction
    {
        [FunctionName("QuestionUpdateTimeFunction")]
        public static async Task Run([TimerTrigger("0 10,40 * * * *")]TimerInfo myTimer,
            [Inject] ICommandBus commandBus,
            ILogger log,

            CancellationToken token)
        {
            log.LogInformation("QuestionUpdateTimeFunction invoke");
            var command = new UpdateQuestionTimeCommand();
            await commandBus.DispatchAsync(command, token);
            log.LogInformation($"QuestionUpdateTimeFunction function executed at: {DateTime.Now}");
        }


        [FunctionName("QuestionSearchSync")]
        public static async Task RunQuestionSearchAsync([TimerTrigger("0 20,50 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            await SyncFunc.StartSearchSync(starter, log, SyncType.Question);
        }



        [FunctionName("QuestionPopulate")]
        public static async Task QuestionPopulateAsync([TimerTrigger("0 */15 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Inject] ICommandBus commandBus,
            [Inject] IQueryBus queryBus,
            ILogger log,
            CancellationToken token)
        {
            var questions = await queryBus.QueryAsync(new AdminFictivePendingQuestionEmptyQuery(), token);
            if (questions.Count > 0)
            {
                var command = new ApproveQuestionCommand(questions.Select(s => s.Id));
                await commandBus.DispatchAsync(command, token);
            }
        }

    }
}
