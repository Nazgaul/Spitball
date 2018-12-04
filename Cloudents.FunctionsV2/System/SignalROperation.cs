using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using SignalRMessage = Microsoft.Azure.WebJobs.Extensions.SignalRService.SignalRMessage;

namespace Cloudents.FunctionsV2.System
{
    public class SignalROperation : ISystemOperation<Core.Message.System.SignalRMessage>
    {
        private readonly ILogger _logger;

        public SignalROperation(ILogger logger)
        {
            _logger = logger;
        }

        public async Task DoOperationAsync(Core.Message.System.SignalRMessage msg,
            IBinder binder, CancellationToken token)
        {

            var signalRMessages = await binder.BindAsync<IAsyncCollector<JObject>>(new SignalRAttribute()
            {
                HubName = "SbHub"
            }, token);

            var p = new SignalRMessage
            {
                Target = "Message",
                Arguments = new object[] {msg.GetData()}
            };

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            var x = JsonConvert.SerializeObject(p, settings);

            _logger.Info($"signalr message {x}");
            await signalRMessages.AddAsync(JObject.Parse(x), token);

        }
    }
}