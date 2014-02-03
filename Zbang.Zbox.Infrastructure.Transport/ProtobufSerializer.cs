using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Transport
{
    public class ProtobufSerializer<T> where T : class
    {
        RuntimeTypeModel m_TypeModel;
        public ProtobufSerializer()
        {
            if (typeof(T).IsDefined(typeof(ProtoBuf.ProtoContractAttribute), false))
            {
               
                m_TypeModel = ProtoBuf.Meta.RuntimeTypeModel.Default;
                return;
            }
            m_TypeModel = ProtoBuf.Meta.TypeModel.Create();
            AddTypeToModel(m_TypeModel);

        }

        private MetaType AddTypeToModel(RuntimeTypeModel typeModel)
        {
            var properties = typeof(T).GetProperties().Select(p => p.Name).OrderBy(name => name);
            return typeModel.Add(typeof(T), true).Add(properties.ToArray());
        }

        public byte[] SerializeData(T model)
        {
            using (var m = new MemoryStream())
            {
                m_TypeModel.Serialize(m, model);
                m.Seek(0, SeekOrigin.Begin);
                return m.ToArray();

            }
        }

        public T DeserializeData(byte[] model)
        {
            using (var ms = new MemoryStream(model))
            {
                var obj = m_TypeModel.Deserialize(ms, null, typeof(T));
                return obj as T;
            }
        }
    }
}
