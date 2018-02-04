using System.IO;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure
{
    public class StreamSerializer : IStreamSerializer
    {
        public byte[] Serialize<T>(T value)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, value);
                return ms.ToArray();
                //tempData[key] = Convert.ToBase64String(ms.ToArray());
            }
        }

        public T DeSerialize<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }
    }
}
