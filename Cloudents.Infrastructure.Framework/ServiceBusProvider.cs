using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Request;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Cloudents.Infrastructure.Framework
{
    [UsedImplicitly]
    public class ServiceBusProvider : IServiceBusProvider, IStartable
    {
        private readonly string _connectionString;

        public ServiceBusProvider(IConfigurationKeys configurationKeys)
        {
            _connectionString = configurationKeys.ServiceBus;
        }

        public Task InsertMessageAsync(BaseEmail message, CancellationToken token)
        {
            var topicSubscription = TopicSubscription.Email;
            return InsertMessageAsync(message, topicSubscription);
        }

        public Task InsertMessageAsync(TalkJsUser message, CancellationToken token)
        {
            var topicSubscription = TopicSubscription.TalkJs;
            return InsertMessageAsync(message, topicSubscription);
        }

        public Task InsertMessageAsync(SmsMessage2 message, CancellationToken token)
        {
            var topicSubscription = TopicSubscription.Sms;
            return InsertMessageAsync(message, topicSubscription);
        }

        public Task InsertMessageAsync(BlockChainInitialBalance message, CancellationToken token)
        {
            var topicSubscription = TopicSubscription.BlockChainInitialBalance;
            return InsertMessageAsync(message, topicSubscription);
        }

        public Task InsertMessageAsync(BlockChainQnaSubmit message, CancellationToken token)
        {
            var topicSubscription = TopicSubscription.BlockChainQnA;
            return InsertMessageAsync(message, topicSubscription);
        }

        public Task InsertMessageAsync(UrlRedirectQueueMessage message, CancellationToken token)
        {
            var topicSubscription = TopicSubscription.UrlRedirect;
            return InsertMessageAsync(message, topicSubscription);
        }

        private Task InsertMessageAsync(object obj, TopicSubscription subscription)
        {
            var topic = TopicClient.CreateFromConnectionString(_connectionString, subscription.Topic);
            var msMessage = new BrokeredMessage(obj)
            {
                Label = subscription.Subscription
            };
            msMessage.Properties["messageType"] = obj.GetType().AssemblyQualifiedName;

            return topic.SendAsync(msMessage);
        }

        public void Start()
        {
            var topicSubscriptions = typeof(TopicSubscription).GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly && f.FieldType == typeof(TopicSubscription))
                .Select(s => s.GetValue(null)).Cast<TopicSubscription>().ToList();

            // PART 1 - CREATE THE TOPIC
            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(_connectionString);

            foreach (var topic in topicSubscriptions.Select(s => s.Topic).Distinct())
            {
                if (!namespaceManager.TopicExists(topic))
                {
                    var td = new TopicDescription(topic);
                    namespaceManager.CreateTopic(td);
                }
            }
            foreach (var topicSubscription in topicSubscriptions)
            {
                if (!namespaceManager.SubscriptionExists(topicSubscription.Topic, topicSubscription.Subscription))
                {
                    namespaceManager.CreateSubscription(topicSubscription.Topic, topicSubscription.Subscription, new SqlFilter($"sys.Label='{topicSubscription.Subscription}'"));
                }
            }
        }
    }
}