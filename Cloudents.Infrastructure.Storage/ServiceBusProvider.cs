using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
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

        private Task InsertTopicMessageAsync(string topic)
        {
            var topicClient = _topicClients.GetOrAdd(topic, x => new TopicClient(_connectionString, x));
            return topicClient.SendAsync(new Message()
            {
                Label = "ram",
                CorrelationId = "ram",
                Body = Encoding.UTF8.GetBytes("this is ram")
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
                ContentType = "application/json"
            });
        }

        public Task InsertMessageAsync(SmsMessage2 message, CancellationToken token)
        {
            return InsertQueueMessageAsync("sms", message,  token);
        }

        //public async Task SendCustomMessageAsync()
        //{

        //    var topic = new Microsoft.Azure.ServiceBus.TopicClient("Endpoint=sb://spitball-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=CACOBTEeKVemCY7ScVBHYXBwDkClQcCKUW7QGq8dNfA=", "topic1");

        //    await topic.SendAsync(new Message()
        //    {
        //        Label = "ram",
        //        CorrelationId = "ram",
        //        Body = Encoding.UTF8.GetBytes("this is ram")
        //    });


        //}
    }
}