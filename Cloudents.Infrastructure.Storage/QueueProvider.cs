using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Storage
{
    //[UsedImplicitly]
    //public class QueueProvider : IQueueProvider
    //{
    //    private readonly CloudQueueClient _queueClient;

    //    public QueueProvider(ICloudStorageProvider storageProvider)
    //    {
    //        _queueClient = storageProvider.GetQueueClient();

    //    }

    //    public Task InsertEmailMessageAsync<T>(T obj, CancellationToken token) where T : QueueEmail
    //    {
    //        return InsertMessageAsync(obj, QueueName.Email, token);
         
    //    }

    //    public Task InsertBackgroundMessageAsync<T>(T obj, CancellationToken token) where T : QueueBackground
    //    {
    //        return InsertMessageAsync(obj, QueueName.Background, token);
    //    }

    //    private Task InsertMessageAsync(object obj, QueueName queueName, CancellationToken token)
    //    {
    //        var queue = _queueClient.GetQueueReference(queueName.Key);

    //        var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
    //        {
    //            TypeNameHandling = TypeNameHandling.Auto
    //        });
    //        var cloudMessage = new CloudQueueMessage(json);
            
    //        return queue.AddMessageAsync(cloudMessage);
    //    }
    //}
}
