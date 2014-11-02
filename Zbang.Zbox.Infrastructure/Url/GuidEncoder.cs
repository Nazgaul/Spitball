using System;

namespace Zbang.Zbox.Infrastructure.Url
{
    public static class GuidEncoder
    {
        public static string Encode(string guidText)
        {
            var guid = new Guid(guidText);
            return Encode(guid);
        }

        public static string Encode(Guid guid)
        {
            //var x = new Base62(500);
            //var v = x.ToString();
            //var z = new Base62(v);
            var enc = Convert.ToBase64String(guid.ToByteArray());

            
            enc = enc.Replace("/", "_");
            enc = enc.Replace("+", "-");
            return enc.Substring(0, 22);
        }

        public static Guid Decode(string encoded)
        {
            if (encoded.Length != 22)
            {
                return Guid.Empty;
            }
            encoded = encoded.Replace("_", "/");
            encoded = encoded.Replace("-", "+");
            var buffer = Convert.FromBase64String(encoded + "==");
            return new Guid(buffer);
        }

        public static Guid? TryParseNullableGuid(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            Guid guid;
            if (Guid.TryParse(str, out guid))
            {
                return guid;
            }
            return Decode(str);

        }
    }
}