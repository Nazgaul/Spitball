using System.IO;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData
{
    public class SerializeData<T> where T : class
    {
        //public byte[] Serialize(T data)
        //{
        //    using (var m = new MemoryStream())
        //    {
        //        var dcs = new DataContractSerializer(typeof(T));                
        //        dcs.WriteObject(m, data);
        //        m.Position = 0;
        //        return m.ToArray();
        //    }
        //}

        //public T Deserialize(byte[] msg)
        //{
        //    using (var ms = new MemoryStream(msg))
        //    {
        //        var dcs = new DataContractSerializer(typeof(T));
        //        T data = dcs.ReadObject(ms) as T;
        //        return data;
        //    }
        //}
       

        //public T DeserializeBinary(byte[] msg)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        var d = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //        ms.Write(msg, 0, msg.Length);
        //        ms.Seek(0, SeekOrigin.Begin);
        //        return d.Deserialize(ms) as T;
        //    }
        //}

        //public byte[] SerializeBinary(T data)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        var d = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //        d.Serialize(ms, data);
        //        return ms.ToArray();
        //    }
        //}

        
    }


}
