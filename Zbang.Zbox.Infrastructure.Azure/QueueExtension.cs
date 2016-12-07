using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Zbang.Zbox.Infrastructure.Azure
{
    public static class QueueExtension
    {
        
        public static Task InsertToQueueProtoAsync<T>(this CloudQueue cloudQueue, T data) where T : class
        {
            using (var m = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(m, data);
                m.Seek(0, SeekOrigin.Begin);
                return cloudQueue.AddMessageAsync(new CloudQueueMessage(m.ToArray()));
            }
        }

        public static Task InsertToQueueProtoAsync<T>(this CloudQueue cloudQueue, T data,CancellationToken token) where T : class
        {
            using (var m = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(m, data);
                m.Seek(0, SeekOrigin.Begin);
                return cloudQueue.AddMessageAsync(new CloudQueueMessage(m.ToArray()), token);
            }
        }

        public static T FromMessageProto<T>(this CloudQueueMessage cloudQueueMessage) where T : class
        {
            if (cloudQueueMessage == null) throw new ArgumentNullException(nameof(cloudQueueMessage));
            using (var ms = new MemoryStream(cloudQueueMessage.AsBytes))
            {
                try
                {
                    return ProtoBuf.Serializer.Deserialize<T>(ms);
                }
                catch (ProtoBuf.ProtoException)
                {
                    return default(T);

                }
            }
        }
    }
}
