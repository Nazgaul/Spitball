using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Core.Storage.Dto;
using Cloudents.FunctionsV2.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class QuestionFunction
    {
        [FunctionName("QuestionUpdateTimeFunction")]
        public static async Task Run([TimerTrigger("0 0 0 1 * *")]TimerInfo myTimer,
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
        public static async Task RunQuestionSearchAsync([TimerTrigger("0 */10 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            await SyncFunc.StartSearchSync(starter, log, SyncType.Question);
        }

       

        [FunctionName("QuestionPopulate")]
        public static async Task QuestionPopulateAsync([TimerTrigger("0 */15 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Queue(QueueName.QuestionsQueueName)] CloudQueue queue,
            [Inject] ICommandBus commandBus,
            ILogger log,
            CancellationToken token)
        {
            log.LogInformation("QuestionPopulate invoke");
            var msg = await queue.GetMessageAsync();
            if (msg == null)
            {
                return;
            }

            var answerMessage = JsonConvert.DeserializeObject<NewQuestionMessage>(msg.AsString);
            var command = new CreateQuestionCommand(
                answerMessage.SubjectId,
                answerMessage.Text,
                answerMessage.Price,
                answerMessage.UserId, null, 
                QuestionColor.Default);
            await commandBus.DispatchAsync(command, token);
            await queue.DeleteMessageAsync(msg);
            log.LogInformation($"QuestionPopulate function executed at: {DateTime.Now}");
        }

    }
}
