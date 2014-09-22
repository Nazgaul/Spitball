using ProtoBuf.Meta;
using System.IO;
using System.Linq;

namespace Zbang.Zbox.Infrastructure.Transport
{
    public class ProtobufSerializer<T> where T : class
    {
        private readonly RuntimeTypeModel m_TypeModel;
        public ProtobufSerializer()
        {
            if (typeof(T).IsDefined(typeof(ProtoBuf.ProtoContractAttribute), false))
            {

                m_TypeModel = RuntimeTypeModel.Default;
                return;
            }
            m_TypeModel = TypeModel.Create();
            AddTypeToModel(m_TypeModel);

        }

        private void AddTypeToModel(RuntimeTypeModel typeModel)
        {
            var properties = typeof(T).GetProperties().Select(p => p.Name).OrderBy(name => name);
            typeModel.Add(typeof(T), true).Add(properties.ToArray());
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

        public T DeSerializeData(byte[] model)
        {
            using (var ms = new MemoryStream(model))
            {
                var obj = m_TypeModel.Deserialize(ms, null, typeof(T));
                return obj as T;
            }
        }
    }
}
