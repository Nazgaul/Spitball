using System.IO;
using System.Linq;
using System.Reflection;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using JetBrains.Annotations;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Cloudents.Infrastructure
{
    [UsedImplicitly]
    public class BinarySerializer : IBinarySerializer
    {
        public BinarySerializer()
        {
            // AppDomain.CurrentDomain.GetAssemblies().ToList()
            foreach (var type in Assembly.Load("Cloudents.Core").GetTypes()
                .Where(w => w.GetCustomAttribute<BinaryIncludeSerializeAttribute>() != null))
            {
                foreach (var att in type.GetCustomAttributes<BinaryIncludeSerializeAttribute>())
                {
                    RuntimeTypeModel.Default.Add(type, true).AddSubType(att.Tag, att.KnownType);

                }


            }


        }
        public byte[] Serialize<T>(T value)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, value);
                return ms.ToArray();
            }
        }

        public T DeSerialize<T>(byte[] input)
        {
            using (var ms = new MemoryStream(input))
            {
                var result = Serializer.Deserialize<T>(ms);
                return result;
            }
        }
    }
}
