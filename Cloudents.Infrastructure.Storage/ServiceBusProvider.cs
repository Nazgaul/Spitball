using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Storage
{
    public class ServiceBusProvider : IServiceBusProvider
    {
        private readonly string _connectionString;

        public ServiceBusProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task InsertMessageAsync(BaseEmail message, CancellationToken token)
        {
            var topicSubscription = TopicSubscription.Email;
            return InsertMessageAsync(message, topicSubscription);
        }

       

        public Task InsertMessageAsync(UrlRedirectQueueMessage message, CancellationToken token)
        {
            var topicSubscription = TopicSubscription.UrlRedirect;
            return InsertMessageAsync(message, topicSubscription);
        }

        public Task InsertMessageAsync(SmsMessage2 message, CancellationToken token)
        {
            var topicSubscription = TopicSubscription.Sms;
            return InsertMessageAsync(message, topicSubscription);
        }


        private Task InsertMessageAsync(object obj, TopicSubscription subscription)
        {
            var topic = new TopicClient(_connectionString,subscription.Topic);
            

           var t =  JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            var msMessage = new Message(Encoding.UTF8.GetBytes(t))
            {
                Label = subscription.Subscription,
                UserProperties = {["messageType"] = obj.GetType().AssemblyQualifiedName}
            };

            return topic.SendAsync(msMessage);
        }
    }
}