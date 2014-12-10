using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CookieTempDataProvider : ITempDataProvider
    {
        private const string TempDataCookieKey = "_temp";
        private readonly HttpContextBase m_HttpContext;
        private readonly Compress m_Compress;

        public CookieTempDataProvider(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            m_HttpContext = httpContext;
            m_Compress = new Compress();
        }
       

        protected virtual IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
        {
            HttpCookie cookie = m_HttpContext.Request.Cookies[TempDataCookieKey];
            if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
            {
                IDictionary<string, object> deserializedDictionary = Base64StringToDictionary2(cookie.Value);

                cookie.Expires = DateTime.MinValue;
                cookie.Value = String.Empty;

                if (m_HttpContext.Response != null && m_HttpContext.Response.Cookies != null)
                {
                    HttpCookie responseCookie = m_HttpContext.Response.Cookies[TempDataCookieKey];
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

            var cookie = new HttpCookie(TempDataCookieKey) {HttpOnly = true, Value = cookieValue};

            m_HttpContext.Response.Cookies.Add(cookie);
        }

        //public IDictionary<string, object> Base64StringToDictionary(string base64EncodedSerializedTempData)
        //{

        //    byte[] bytes = Convert.FromBase64String(base64EncodedSerializedTempData);
        //    var decompressBytes = m_Compress.DecompressFromGzip(bytes);
        //    using (var memStream = new MemoryStream(decompressBytes))
        //    {
        //        var binFormatter = new BinaryFormatter();
        //        return binFormatter.Deserialize(memStream, null) as IDictionary<string, object>;
        //    }
        //}

        //public string DictionaryToBase64String(IDictionary<string, object> values)
        //{
        //    using (var memStream = new MemoryStream())
        //    {
        //        memStream.Seek(0, SeekOrigin.Begin);
        //        var binFormatter = new BinaryFormatter();
        //        binFormatter.Serialize(memStream, values);
        //        memStream.Seek(0, SeekOrigin.Begin);
        //        byte[] bytes = memStream.ToArray();
        //        var compressBytes = m_Compress.CompressToGzip(bytes);

        //        var y = Convert.ToBase64String(compressBytes);
        //        return y;
        //    }
        //}

        public string DictionaryToBase64String2(IDictionary<string, object> values)
        {
            try
            {
                var s = new JavaScriptSerializer();
                var dataAsString = s.Serialize(values);
                var bytes = GetBytes(dataAsString);
                var compressBytes = m_Compress.CompressToGzip(bytes);
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
                var decompressBytes = m_Compress.DecompressFromGzip(bytes);
                var dataAsString = GetString(decompressBytes);
                var s = new JavaScriptSerializer();
                return s.Deserialize<IDictionary<string, object>>(dataAsString);
            }
            catch
            {
                return null;
            }
        }
/*
        /// <summary>
        /// Converts the byte array to a string.
        /// </summary>
        public string Bytes2String(byte[] bytes)
        {
            var ms = new MemoryStream(bytes.Length);

            foreach (byte t in bytes)
            {
// if it is a zero, or if it would make a surrogate double byte, then escape it with an extra zero
                if (t == 0 || (0xd8 <= t && t <= 0xdf && ms.Length % 2 == 1))
                    ms.WriteByte(0);
                ms.WriteByte(t);
            }

            // make sure the length is even
            if (ms.Length % 2 == 1)
                ms.WriteByte(0);

            // UTF-16 LE decoding
            return Encoding.Unicode.GetString(ms.ToArray());
        }
*/

        byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        string GetString(byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

/*
        /// <summary>
        /// Converts the string to a byte array.
        /// </summary>
        public  byte[] String2Bytes(string s)
        {
            // UTF-16 LE encoding
            var bytes = Encoding.Unicode.GetBytes(s);

            var ms = new MemoryStream(bytes.Length);

            var escaped = false;
            foreach (byte t in bytes)
            {
// if it is a non-escaped zero, then treat it as an escape byte (may be the last byte as well)
                if (t == 0 && !escaped)
                {
                    escaped = true;
                }
                else
                {
                    escaped = false;
                    ms.WriteByte(t);
                }
            }

            return ms.ToArray();
        }
*/



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