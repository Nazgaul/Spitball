using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Azure
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

        //public static void InsertToQueueProto<T>(this CloudQueue cloudQueue, T data) where T : class
        //{
        //    if (cloudQueue == null) throw new ArgumentNullException(nameof(cloudQueue));
        //    using (var m = new MemoryStream())
        //    {
        //        ProtoBuf.Serializer.Serialize(m, data);
        //        m.Seek(0, SeekOrigin.Begin);
        //        cloudQueue.AddMessage(new CloudQueueMessage(m.ToArray()));

        //    }
        //}



        //public static Task DeleteMessagesAsync(this CloudQueue cloudQueue, IEnumerable<CloudQueueMessage> messages)
        //{
        //    if (messages == null)
        //    {
        //        return Task.FromResult(false);
        //    }
        //    var cloudQueueMessages = messages as CloudQueueMessage[] ?? messages.ToArray();
        //    if (!cloudQueueMessages.Any())
        //    {
        //        return Task.FromResult(false);
        //    }
        //    var list = cloudQueueMessages.Select(cloudQueue.DeleteMessageAsync).ToList();
        //    return Task.WhenAll(list);
        //}

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
    public static Task InsertToQueueJsonAsync<T>(this CloudQueue cloudQueue, T data) where T : class
        {
            var serializedObject = JsonConvert.SerializeObject(data, Settings);
            return cloudQueue.AddMessageAsync(new CloudQueueMessage(serializedObject));
        }
        public static Task InsertToQueueProtoAsync<T>(this CloudQueue cloudQueue, T data) where T : class
        {
            using (var m = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(m, data);
                m.Seek(0, SeekOrigin.Begin);
                return cloudQueue.AddMessageAsync(new CloudQueueMessage(m.ToArray()));

            }
        }

        public static T FromMessageJson<T>(this CloudQueueMessage cloudQueueMessage) where T : class
        {
            return JsonConvert.DeserializeObject<T>(cloudQueueMessage.AsString, Settings);
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
