using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Chat;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Request;
using Cloudents.Core.Storage;
using Cloudents.Functions.Di;
using Cloudents.Infrastructure.Framework;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;

namespace Cloudents.Functions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure function")]
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
                log.Error($"invalid talkJs user got userId {obj.Id} but no user name");
                return;
            }
            log.Info($"processing {obj.Id}");

            var user = new User(obj.Name)
            {
                Email = obj.Email == null ? null : new[] { obj.Email },
                Phone = obj.Phone == null ? null : new[] { obj.Phone },
                PhotoUrl = obj.PhotoUrl,

            };
            var secret = InjectConfiguration.GetEnvironmentVariable("TalkJsSecret");
            var appId = InjectConfiguration.GetEnvironmentVariable("TalkJsAppId");
            var t = await chatService.PutJsonAsync(
                new Uri($"https://api.talkjs.com/v1/{appId}/users/{obj.Id}"),
                user, new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization",$"Bearer {secret}")
                }, token).ConfigureAwait(false);
            if (!t)
            {
                log.Error("cannot send talkjs user " + obj.Id);
            }
        }

        //[FunctionName("BlockChainInitialBalance")]
        public static async Task BlockChainInitialBalanceAsync(
            [ServiceBusTrigger(TopicSubscription.Background, nameof(TopicSubscription.BlockChainInitialBalance))]
            BlockChainInitialBalance obj,
            [Inject] IBlockChainErc20Service service,
            TraceWriter log,
            CancellationToken token)
        {
            await service.SetInitialBalanceAsync(obj.PublicAddress, token).ConfigureAwait(false);
            log.Info("Initial balance success");
        }

       // [FunctionName("BlockChainQna")]
        public static async Task BlockChainQnaAsync(
            [ServiceBusTrigger(TopicSubscription.Background, nameof(TopicSubscription.BlockChainQnA))]
            BrokeredMessage obj,
            [Inject] IBlockChainQAndAContract service,
            TraceWriter log,
            CancellationToken token)
        {
            if (obj.DeliveryCount > 3)
            {
                return;
            }
            var qnaObject = obj.GetBodyInheritance<BlockChainQnaSubmit>();
            await service.SubmitAsync((dynamic)qnaObject, token).ConfigureAwait(false);
            log.Info("success");
        }
    }
}

