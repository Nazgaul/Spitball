using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Storage
{
    public class ServiceBusProvider : IServiceBusProvider
    {
        private readonly string _connectionString;

        private readonly ConcurrentDictionary<string, TopicClient> _topicClients = new ConcurrentDictionary<string, TopicClient>();
        private readonly ConcurrentDictionary<string, QueueClient> _queueClients = new ConcurrentDictionary<string, QueueClient>();

        public ServiceBusProvider(IConfigurationKeys keys)
        {
            _connectionString = keys.ServiceBus;

        }

        private Task InsertTopicMessageAsync(string topic, object obj, CancellationToken token)
        {
            var topicClient = _topicClients.GetOrAdd(topic, x => new TopicClient(_connectionString, x));

            //For now
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            token.ThrowIfCancellationRequested();
            return topicClient.SendAsync(new Message()
            {
                Body = Encoding.UTF8.GetBytes(json),
                ContentType = "application/json",
                UserProperties = { ["messageType"] = obj.GetType().AssemblyQualifiedName }
            });
        }

        private Task InsertQueueMessageAsync(string queue, object obj, CancellationToken token)
        {
            var queueClient = _queueClients.GetOrAdd(queue, x => new QueueClient(_connectionString, x));

            var json = JsonConvert.SerializeObject(obj);

            token.ThrowIfCancellationRequested();
            return queueClient.SendAsync(new Message()
            {
                Body = Encoding.UTF8.GetBytes(json),
                ContentType = "application/json",
                UserProperties = { ["messageType"] = obj.GetType().AssemblyQualifiedName }

            });
        }

        public Task InsertMessageAsync(SmsMessage2 message, CancellationToken token)
        {
            return InsertQueueMessageAsync("sms", message, token);
        }

        public Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token)
        {
            return InsertTopicMessageAsync("background2", obj, token);
        }

    }
}