using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using SignalRMessage = Microsoft.Azure.WebJobs.Extensions.SignalRService.SignalRMessage;

namespace Cloudents.FunctionsV2.System
{
    public class SignalROperation : ISystemOperation<Cloudents.Core.Message.System.SignalRMessage>
    {
        public async Task DoOperationAsync(Cloudents.Core.Message.System.SignalRMessage msg, IBinder binder, CancellationToken token)
        {
            
            var signalRMessages = await binder.BindAsync<IAsyncCollector<SignalRMessage>>(new SignalRAttribute()
            {
                HubName = "SbHub"
            }, token);

            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "Message",
                    Arguments = new object[] { msg.GetData() }
                }, token);

        }
    }
}