using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Storage
{
    public class ServiceBusProvider : IServiceBusProvider
    {
        private readonly string _connectionString;

        //private readonly ConcurrentDictionary<string, TopicClient> _topicClients = new ConcurrentDictionary<string, TopicClient>();
        private readonly ConcurrentDictionary<string, QueueClient> _queueClients = new ConcurrentDictionary<string, QueueClient>();

        public ServiceBusProvider(IConfigurationKeys keys)
        {
            _connectionString = keys.ServiceBus;

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
            });
        }

        public Task InsertMessageAsync(SmsMessage2 message, CancellationToken token)
        {
            return InsertQueueMessageAsync("sms", message, token);
        }

        public Task InsertMessageAsync(SignalRTransportType obj, string groupId, CancellationToken token)
        {
            return InsertMessageAsync(obj, null, groupId, token);

        }

        public Task InsertMessageAsync(SignalRTransportType obj, CancellationToken token)
        {
            return InsertMessageAsync(obj, null, null, token);
        }
        public Task InsertMessageAsync(SignalRTransportType obj, long? userId, CancellationToken token)
        {
            return InsertMessageAsync(obj, userId, null, token);
        }


        private Task InsertMessageAsync(SignalRTransportType obj, long? userId, string group, CancellationToken token)
        {
            var queueClient = _queueClients.GetOrAdd("signalr", x => new QueueClient(_connectionString, x));

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            jsonSerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            var json = JsonConvert.SerializeObject(obj, jsonSerializerSettings);

            token.ThrowIfCancellationRequested();
            return queueClient.SendAsync(new Message
            {
                Body = Encoding.UTF8.GetBytes(json),
                UserProperties =
                {
                    ["userId"] = userId,
                    ["group"] = group
                },
                ContentType = "application/json",
            });
        }

        //public Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token)
        //{
        //    return InsertTopicMessageAsync("background2", obj, token);
        //}


        //private Task InsertTopicMessageAsync(string topic, object obj, CancellationToken token)
        //{
        //    var topicClient = _topicClients.GetOrAdd("background2", x => new TopicClient(_connectionString, x));

        //    var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        //    {
        //        NullValueHandling = NullValueHandling.Ignore,
        //        //We need this for now because other wise inner mapping wont convert to object
        //        TypeNameHandling = TypeNameHandling.All
        //    });

        //    token.ThrowIfCancellationRequested();
        //    return topicClient.SendAsync(new Message()
        //    {
        //        Body = Encoding.UTF8.GetBytes(json),
        //        ContentType = "application/json",
        //        UserProperties = { ["messageType"] = obj.GetType().FullName }
        //    });
        //}
    }
}