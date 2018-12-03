using Microsoft.ServiceBus;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ConsoleApp
{
    public class CreateServiceBus
    {
        public static void Create()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(
                "Endpoint=sb://spitball-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=CACOBTEeKVemCY7ScVBHYXBwDkClQcCKUW7QGq8dNfA=");
            var topics = namespaceManager.GetSubscriptions("topic1");
            //var subscriptions = namespaceManager.GetSubscriptions();
            SubscriptionDescription subscription = namespaceManager.CreateSubscription("topic1", "temp3");
            //Filter filter = new SqlFilter("From LIKE '%Smith'");
            Filter f = new CorrelationFilter()
            {
                Label = "ram"
            };
            //SubscriptionDescription description, Filter filter
            namespaceManager.CreateSubscription("topic1", "xxx", f);

        }
    }
}