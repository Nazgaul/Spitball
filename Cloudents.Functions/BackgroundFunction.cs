using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Chat;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Functions.Di;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Functions
{
    public static class BackgroundFunction
    {
        [FunctionName("FunctionTalkJs")]
        public static async Task BackgroundFunctionAsync(
            [ServiceBusTrigger(TopicSubscription.Background, nameof(TopicSubscription.TalkJs))]TalkJsUser obj,
            [Inject] IRestClient chatService,
            TraceWriter log,
            CancellationToken token)
        {
            if (obj.Name == null)
            {
                return;
            }
            var user = new User(obj.Name)
            {
                Email = obj.Email == null ? null : new[] { obj.Email },
                Phone = obj.Phone == null ? null : new[] { obj.Phone },
                PhotoUrl = obj.PhotoUrl,
                
            };
            var secret = InjectConfiguration.GetEnvironmentVariable("TalkJsSecret");
            var t = await chatService.PutJsonAsync(
                new Uri($"https://api.talkjs.com/v1/tXsrQpOx/users/{obj.Id}"),
                user, new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization",$"Bearer {secret}")
                }, token).ConfigureAwait(false);
            if (!t)
            {
                log.Error("cannot send talkjs user " + obj.Id);
            }
        }
    }
}
