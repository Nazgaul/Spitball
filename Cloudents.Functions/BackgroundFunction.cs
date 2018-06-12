using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Chat;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Functions.Di;
using Microsoft.Azure.WebJobs;

namespace Cloudents.Functions
{
    public static class BackgroundFunction
    {
        [FunctionName("FunctionTalkJs")]
        public static async Task BackgroundFunctionAsync(
            [ServiceBusTrigger(TopicSubscription.Background, nameof(TopicSubscription.TalkJs))]TalkJsUser obj,
            [Inject] IRestClient chatService,
            CancellationToken token)
        {
            var user = new User
            {
                Email = obj.Email == null ? null : new[] { obj.Email },
                Name = obj.Name,
                Phone = obj.Phone == null ? null : new[] { obj.Phone },
                PhotoUrl = obj.PhotoUrl,
                
            };
            await chatService.PutJsonAsync(
                new Uri($"https://api.talkjs.com/v1/tXsrQpOx/users/{obj.Id}"),
                user, new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization","Bearer sk_test_AQGzQ2Rlj0NeiNOEdj1SlosU")
                }, token).ConfigureAwait(false);

        }
    }
}
