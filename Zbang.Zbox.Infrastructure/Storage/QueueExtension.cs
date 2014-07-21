//using Microsoft.WindowsAzure.StorageClient;

using System;
using Microsoft.WindowsAzure.Storage.Queue;
using System.IO;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public static class QueueExtension
    {
        //public static void InsertToQueue<T>(this CloudQueue cloudQueue, T data) where T : class
        //{
        //    using (var m = new MemoryStream())
        //    {
        //        var dcs = new DataContractSerializer(typeof(T));
        //        dcs.WriteObject(m, data);
        //        m.Position = 0;
        //        cloudQueue.AddMessageAsync(new CloudQueueMessage(m.ToArray()));

        //    }
        //}
        //public static T FromMessage<T>(this CloudQueueMessage cloudQueueMessage) where T : class
        //{
        //    using (var ms = new MemoryStream(cloudQueueMessage.AsBytes))
        //    {
        //        var dcs = new DataContractSerializer(typeof(T));
        //        var data = dcs.ReadObject(ms) as T;
        //        return data;
        //    }
        //}

        public static void InsertToQueueProto<T>(this CloudQueue cloudQueue, T data) where T : class
        {
            if (cloudQueue == null) throw new ArgumentNullException("cloudQueue");
            using (var m = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(m, data);
                m.Seek(0, SeekOrigin.Begin);
                cloudQueue.AddMessage(new CloudQueueMessage(m.ToArray()));

            }
        }

        public static async Task InsertToQueueProtoAsync<T>(this CloudQueue cloudQueue, T data) where T : class
        {
            using (var m = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(m, data);
                m.Seek(0, SeekOrigin.Begin);
                await cloudQueue.AddMessageAsync(new CloudQueueMessage(m.ToArray()));
                
            }
        }
        public static T FromMessageProto<T>(this CloudQueueMessage cloudQueueMessage) where T : class
        {
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

                //var dcs = new DataContractSerializer(typeof(T));
                //T data = dcs.ReadObject(ms) as T;
                //return data;
            }
        }

    }
}
