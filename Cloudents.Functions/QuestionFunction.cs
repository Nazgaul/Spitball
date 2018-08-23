using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Core.Storage.Dto;
using Cloudents.Functions.Di;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Cloudents.Functions
{
    public static class QuestionFunction
    {
        [FunctionName("QuestionUpdateTimeFunction")]
        public static async Task Run([TimerTrigger("0 0 1 * * *")]TimerInfo myTimer,
            [Inject] ICommandBus commandBus,
            TraceWriter log,
            CancellationToken token)
        {
            log.Info("QuestionUpdateTimeFunction invoke");
            var command = new UpdateQuestionTimeCommand();
            await commandBus.DispatchAsync(command, token);
            log.Info($"QuestionUpdateTimeFunction function executed at: {DateTime.Now}");
        }

        [FunctionName("QuestionPopulate")]
        public static async Task QuestionPopulateAsync([TimerTrigger("0 */15 * * * *",RunOnStartup = true)]TimerInfo myTimer,
           // [Queue(QueueName.QuestionsQueueName)] NewQuestionMessage answerMessage,
           [Queue(QueueName.QuestionsQueueName)] CloudQueue queue,

            [Inject] ICommandBus commandBus,
            TraceWriter log,
            CancellationToken token)
        {
            log.Info("QuestionPopulate invoke");

            var msg = await queue.GetMessageAsync(token);
            if (msg == null)
            {
                return;
            }

            var answerMessage = JsonConvert.DeserializeObject<NewQuestionMessage>(msg.AsString);
            var command = new CreateQuestionCommand
            {
                Price = answerMessage.Price,
                SubjectId = answerMessage.SubjectId,
                Text = answerMessage.Text,
                UserId = answerMessage.UserId
            };
            await commandBus.DispatchAsync(command, token);
            log.Info($"QuestionPopulate function executed at: {DateTime.Now}");
        }

    }
}
