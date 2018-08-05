using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Functions.Di;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

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
    }
}
