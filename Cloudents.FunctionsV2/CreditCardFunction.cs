using System.Diagnostics.CodeAnalysis;
using Cloudents.Command;
using Cloudents.Command.Command;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public class CreditCardFunction
    {
        [FunctionName("CreditCardExpired")]
        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting", Justification = "Entry point")]
        public static async Task RunAsync([TimerTrigger("0 0 0 28-31 * *")]TimerInfo myTimer,
            [Inject] ICommandBus commandBus,
            CancellationToken token)
        {
            var command = new UpdateCreditCardExpiredCommand();
            await commandBus.DispatchAsync(command, token);

        }
    }
}
