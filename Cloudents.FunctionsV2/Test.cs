using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class Test
    {
      //  [FunctionName("Test")]
        public static async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Inject] IQueryBus queryBus2,
            ILogger log,
            CancellationToken token)
        {
            log.LogInformation("Starting Test v2");
            var query = new UserDataByIdQuery(638);
            var data = await queryBus2.QueryAsync<UserAccountDto>(query, token);
            log.LogInformation(data.Id.ToString());
            //var command = new UpdateQuestionTimeCommand();
            //await commandBus.DispatchAsync(command, token);
            log.LogInformation($"Test C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
