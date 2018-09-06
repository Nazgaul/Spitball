using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using Cloudents.Core.Storage.Dto;
using Cloudents.Functions.Di;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions
{
    public static class QuestionFunction
    {
        [FunctionName("QuestionUpdateTimeFunction")]
        public static async Task Run([TimerTrigger("0 0 0 1 * *")]TimerInfo myTimer,
            [Inject] ICommandBus commandBus,
            TraceWriter log,
            CancellationToken token)
        {
            log.Info("QuestionUpdateTimeFunction invoke");
            var command = new UpdateQuestionTimeCommand();
            await commandBus.DispatchAsync(command, token);
            log.Info($"QuestionUpdateTimeFunction function executed at: {DateTime.Now}");
        }


        [FunctionName("QuestionSearchSync")]
        public static async Task RunQuestionSearchAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
            [Blob("spitball/AzureSearch/question-version.txt", FileAccess.ReadWrite)]
            CloudBlockBlob blob,
            [Inject]
            IQueryBus bus,
            [Inject] ISearchServiceWrite<Question> searchServiceWrite,
            TraceWriter log,
            CancellationToken token)
        {
            //var (update, delete, version) = await bus.QueryAsync<(IEnumerable<QuestionAzureSyncDto> update, IEnumerable<long> delete, long version)>(query, token);
            await SyncFunc.SyncAsync2(blob, searchServiceWrite, s =>
                {
                    s.Prefix = s.Text;
                    return s;
                }, q => bus.QueryAsync(q, token),
                log, token);
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        [FunctionName("QuestionPopulate")]
        public static async Task QuestionPopulateAsync([TimerTrigger("0 */15 * * * *", RunOnStartup = true)]TimerInfo myTimer,
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
            await queue.DeleteMessageAsync(msg, token);
            log.Info($"QuestionPopulate function executed at: {DateTime.Now}");
        }

    }
}
