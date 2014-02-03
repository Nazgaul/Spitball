using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Zbang.Zbox.Infrastructure.Url
{
    public class EncryptObject : IEncryptObject
    {

        public string EncryptElement<T>(T obj, params string[] purposes) where T : class
        {
            Infrastructure.Transport.ProtobufSerializer<T> formatter = new Transport.ProtobufSerializer<T>();
            var serializedObj = formatter.SerializeData(obj);
            var bData = MachineKey.Protect(serializedObj, purposes);
            return HttpServerUtility.UrlTokenEncode(bData);

        }

        public T DecryptElement<T>(string hash, params string[] purposes) where T : class
        {
            var bData = HttpServerUtility.UrlTokenDecode(hash);
            var serializedObj = MachineKey.Unprotect(bData, purposes);
            Infrastructure.Transport.ProtobufSerializer<T> formatter = new Transport.ProtobufSerializer<T>();
            return formatter.DeserializeData(serializedObj);
        }

    }

    public interface IEncryptObject 
    {
        string EncryptElement<T>(T obj, params string[] purposes) where T : class;
        T DecryptElement<T>(string hash, params string[] purposes) where T : class;
    }
}
