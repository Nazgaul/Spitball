using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Chat;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Functions.Di;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace Cloudents.Functions
{
    public static class BackgroundFunction
    {
        [FunctionName("BackgroundFunction")]
        public static async Task BackgroundFunctionAsync(
            [QueueTrigger(QueueName.BackgroundName)] string queueMessage,
            [Inject] IChat chatService,
            TraceWriter log,
            CancellationToken token)
        {
            var obj = JsonConvert.DeserializeObject<TalkJsUser>(queueMessage, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            if (obj == null)
            {
                log.Error("error deSerializing");
                return;
            }

            var user = new User
            {
                Email = obj.Email == null ? null : new[] {obj.Email},
                Name = obj.Name,
                Phone = obj.Phone == null ? null : new[] {obj.Phone},
                PhotoUrl = obj.PhotoUrl,

            };

            await chatService.CreateOrUpdateUserAsync(obj.Id, user,token).ConfigureAwait(false);
        }
    }
}
