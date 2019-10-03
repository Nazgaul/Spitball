using Cloudents.Command;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public class CreditCardFunction
    {
        [FunctionName("CreditCardExpired")]
        public static async Task Run([TimerTrigger("0 0 0 28-31 * *")]TimerInfo myTimer,
            [Inject] ICommandBus commandBus,
            CancellationToken token)
        {
            var command = new UpdateCreditCardExpiredCommand();
            await commandBus.DispatchAsync(command, token);

        }
    }
}
