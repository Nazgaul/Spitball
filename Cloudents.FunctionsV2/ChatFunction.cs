using Cloudents.Command;
using Cloudents.Command.Command;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class ChatFunction
    {

        [FunctionName("UpdateChatRoomAdminStatus")]
        public static async Task UpdateChatRoomAdminStatusAsync(
            [TimerTrigger("0 0 0 * * *")]TimerInfo myTimer,
            [Inject] ICommandBus commandBus,
            CancellationToken token
        )
        {
            var command = new UpdateChatRoomAdminStatusCommand();
            await commandBus.DispatchAsync(command, token);
        }
    }
}
