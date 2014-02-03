using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CookieTempDataProvider : ITempDataProvider
    {
        internal const string TempDataCookieKey = "_temp";
        private HttpContextBase _httpContext;
        private readonly Compress _Compress;

        public CookieTempDataProvider(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            _httpContext = httpContext;
            _Compress = new Compress();
        }

        public HttpContextBase HttpContext
        {
            get { return _httpContext; }
        }

        protected virtual IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
        {
            HttpCookie cookie = _httpContext.Request.Cookies[TempDataCookieKey];
            if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
            {
                IDictionary<string, object> deserializedDictionary = Base64StringToDictionary2(cookie.Value);

                cookie.Expires = DateTime.MinValue;
                cookie.Value = String.Empty;

                if (_httpContext.Response != null && _httpContext.Response.Cookies != null)
                {
                    HttpCookie responseCookie = _httpContext.Response.Cookies[TempDataCookieKey];
                    if (responseCookie != null)
                    {
                        responseCookie.Expires = DateTime.MinValue;
                        responseCookie.Value = String.Empty;
                    }
                }

                return deserializedDictionary;
            }

            return new Dictionary<string, object>();
        }

        protected virtual void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values)
        {
            if (values.Count == 0)
            {
                return;
            }
            string cookieValue = DictionaryToBase64String2(values);

            var cookie = new HttpCookie(TempDataCookieKey);
            cookie.HttpOnly = true;
            cookie.Value = cookieValue;

            _httpContext.Response.Cookies.Add(cookie);
        }

        public IDictionary<string, object> Base64StringToDictionary(string base64EncodedSerializedTempData)
        {
            //var pformatter = new Zbang.Zbox.Infrastructure.Transport.ProtobufSerializer<IDictionary<string, object>>();
            //var bytes = Convert.FromBase64String(base64EncodedSerializedTempData);
            //return pformatter.DeserializeData(bytes);


            byte[] bytes = Convert.FromBase64String(base64EncodedSerializedTempData);
            var decompressBytes = _Compress.DecompressFromGzip(bytes);
            using (var memStream = new MemoryStream(decompressBytes))
            {
                var binFormatter = new BinaryFormatter();
                return binFormatter.Deserialize(memStream, null) as IDictionary<string, object>;
            }
        }

        public string DictionaryToBase64String(IDictionary<string, object> values)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                memStream.Seek(0, SeekOrigin.Begin);
                var binFormatter = new BinaryFormatter();
                binFormatter.Serialize(memStream, values);
                memStream.Seek(0, SeekOrigin.Begin);
                byte[] bytes = memStream.ToArray();
                var x = Convert.ToBase64String(bytes);
                var compressBytes = _Compress.CompressToGzip(bytes);

                var y = Convert.ToBase64String(compressBytes);
                return y;
            }
        }

        public string DictionaryToBase64String2(IDictionary<string, object> values)
        {
            try
            {
                var s = new JavaScriptSerializer();
                var dataAsString = s.Serialize(values);
                var bytes = GetBytes(dataAsString);
                var compressBytes = _Compress.CompressToGzip(bytes);
                var y = Convert.ToBase64String(compressBytes);
                var z = HttpServerUtility.UrlTokenEncode(compressBytes);
                return z;
            }
            catch
            {
                return null;
            }
        }

        public IDictionary<string, object> Base64StringToDictionary2(string base64EncodedSerializedTempData)
        {
            try
            {
                //byte[] bytes = Convert.FromBase64String(base64EncodedSerializedTempData);
                var bytes = HttpServerUtility.UrlTokenDecode(base64EncodedSerializedTempData);
                var decompressBytes = _Compress.DecompressFromGzip(bytes);
                var dataAsString = GetString(decompressBytes);
                var s = new JavaScriptSerializer();
                return s.Deserialize<IDictionary<string, object>>(dataAsString);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Converts the byte array to a string.
        /// </summary>
        public string Bytes2String(byte[] bytes)
        {
            var ms = new MemoryStream(bytes.Length);

            for (var i = 0; i < bytes.Length; i++)
            {
                // if it is a zero, or if it would make a surrogate double byte, then escape it with an extra zero
                if (bytes[i] == 0 || (0xd8 <= bytes[i] && bytes[i] <= 0xdf && ms.Length % 2 == 1))
                    ms.WriteByte(0);
                ms.WriteByte(bytes[i]);
            }

            // make sure the length is even
            if (ms.Length % 2 == 1)
                ms.WriteByte(0);

            // UTF-16 LE decoding
            return Encoding.Unicode.GetString(ms.ToArray());
        }

        byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        /// <summary>
        /// Converts the string to a byte array.
        /// </summary>
        public  byte[] String2Bytes(string s)
        {
            // UTF-16 LE encoding
            var bytes = Encoding.Unicode.GetBytes(s);

            var ms = new MemoryStream(bytes.Length);

            var escaped = false;
            for (var i = 0; i < bytes.Length; i++)
            {
                // if it is a non-escaped zero, then treat it as an escape byte (may be the last byte as well)
                if (bytes[i] == 0 && !escaped)
                {
                    escaped = true;
                }
                else
                {
                    escaped = false;
                    ms.WriteByte(bytes[i]);
                }
            }

            return ms.ToArray();
        }



        IDictionary<string, object> ITempDataProvider.LoadTempData(ControllerContext controllerContext)
        {
            return LoadTempData(controllerContext);
        }

        void ITempDataProvider.SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values)
        {
            SaveTempData(controllerContext, values);
        }
    }
}