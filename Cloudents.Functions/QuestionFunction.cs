using Cloudents.Core.Command;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Core.Storage.Dto;
using Cloudents.Functions.Di;
using Cloudents.Infrastructure.Database.Query;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
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

        [FunctionName("QuestionRemoveDuplicatePendingQuestion")]
        public static async Task QuestionRemoveDuplicatePendingQuestion(
            [TimerTrigger("0 */20 * * * *", RunOnStartup = true)]TimerInfo timer,
            [Inject] ReadonlyStatelessSession session,
            [Inject] ICommandBus bus,
            CancellationToken token
        )
        {
            var query = session.Session.CreateSQLQuery(@"WITH CTE AS(
            SELECT id,
                RN = ROW_NUMBER()OVER(PARTITION BY Text ORDER BY Text)
            from sb.question where State = 'pending'
                    )
                select id  FROM CTE WHERE RN > 1");
            var ids = await query.ListAsync<long>(token);
            foreach (var id in ids)
            {
                var command = new Core.Command.Admin.DeleteQuestionCommand(id);
                await bus.DispatchAsync(command, token);
            }

            var updateQuery = session.Session.CreateSQLQuery(@"update sb.[user] 
set balance = (Select sum(price) from sb.[Transaction] where User_id = sb.[User].id)
where balance != (Select sum(price) from sb.[Transaction] where User_id = sb.[User].id)");

            var z = await updateQuery.ExecuteUpdateAsync(token);

        }


        //[FunctionName("QuestionSearchSync")]
        //public static async Task RunQuestionSearchAsync([TimerTrigger("0 */30 * * * *", RunOnStartup = true)] TimerInfo myTimer,
        //    [OrchestrationClient] DurableOrchestrationClient starter,
        //    TraceWriter log)
        //{
        //    await SyncFunc.StartSearchSync(starter, log, SyncType.Question);
        //}



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
            var command = new CreateQuestionCommand(
                answerMessage.SubjectId,
                answerMessage.Text,
                answerMessage.Price,
                answerMessage.UserId, null,
                QuestionColor.Default);
            await commandBus.DispatchAsync(command, token);
            await queue.DeleteMessageAsync(msg, token);
            log.Info($"QuestionPopulate function executed at: {DateTime.Now}");
        }

    }
}
