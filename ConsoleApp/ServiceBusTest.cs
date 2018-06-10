using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ConsoleApp
{
    public class ServiceBusTest
    {
        public void Test()
        {
            string connectionString =
                "Endpoint=sb://spitball-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=CACOBTEeKVemCY7ScVBHYXBwDkClQcCKUW7QGq8dNfA=";

            const string topicName = "ExampleTestTopic";
            const string subscriptionName = "AllMessages";
            const string sub1Name = "Filtered1";
            const string sub2Name = "Filtered2";

            // PART 1 - CREATE THE TOPIC
            Console.WriteLine("Creating the topic");
            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(connectionString);


            if (!namespaceManager.TopicExists(topicName))
            {
                // Configure Topic Settings.
                var td = new TopicDescription(topicName);
                td.MaxSizeInMegabytes = 1024;
                td.DefaultMessageTimeToLive = TimeSpan.FromMinutes(5);

                namespaceManager.CreateTopic(td);
            }

            if (!namespaceManager.SubscriptionExists(topicName, subscriptionName))
            {
                namespaceManager.CreateSubscription(topicName, subscriptionName);
            }
            if (namespaceManager.SubscriptionExists(topicName, sub1Name))
            {
                Console.WriteLine("Deleting subscription {0}", sub1Name);
                namespaceManager.DeleteSubscription(topicName, sub1Name);
            }
            Console.WriteLine("Creating subscription {0}", sub1Name);
            namespaceManager.CreateSubscription(topicName, sub1Name, new SqlFilter("From LIKE '%Smith'"));
            if (namespaceManager.SubscriptionExists(topicName, sub2Name))
            {
                Console.WriteLine("Deleting subscription {0}", sub2Name);
                namespaceManager.DeleteSubscription(topicName, sub2Name);
            }
            Console.WriteLine("Creating subscription {0}", sub2Name);
            namespaceManager.CreateSubscription(topicName, sub2Name, new SqlFilter("sys.Label='important'"));
        }
    }
}