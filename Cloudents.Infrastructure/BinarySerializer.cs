using System.IO;
using Cloudents.Core.Interfaces;
using ProtoBuf;

namespace Cloudents.Infrastructure
{
    public class BinarySerializer : IBinarySerializer
    {
        public void Serialize(Stream sr, object o)
        {
            Serializer.Serialize(sr, o);

        }

        public T Deserialize<T>(Stream sr)
        {
            return Serializer.Deserialize<T>(sr);
        }

        public byte[] Serialize(object o)
        {
            using (var ms = new MemoryStream())
            {
                Serialize(ms, o);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] sr)
        {
            using (var ms = new MemoryStream(sr))
            {
                return Deserialize<T>(sr: ms);
            }
        }
    }

    
}