using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Cloudents.Mvc2Jared.Helper
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
                throw new ArgumentNullException(nameof(httpContext));
            }
            m_HttpContext = httpContext;
            m_Compress = new Compress();
        }


        protected virtual IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
        {
            HttpCookie cookie = m_HttpContext.Request.Cookies[TempDataCookieKey];
            if (!string.IsNullOrEmpty(cookie?.Value))
            {
                IDictionary<string, object> deserializedDictionary = Base64StringToDictionary2(cookie.Value);

                cookie.Expires = DateTime.MinValue;
                cookie.Value = string.Empty;

                HttpCookie responseCookie = m_HttpContext.Response?.Cookies?[TempDataCookieKey];
                if (responseCookie != null)
                {
                    responseCookie.Expires = DateTime.MinValue;
                    responseCookie.Value = string.Empty;
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

            var cookie = new HttpCookie(TempDataCookieKey) { HttpOnly = true, Value = cookieValue };

            m_HttpContext.Response.Cookies.Add(cookie);
        }

        public string DictionaryToBase64String2(IDictionary<string, object> values)
        {
            try
            {
                // var s = new JavaScriptSerializer();
                string json = JsonConvert.SerializeObject(values);
                //var dataAsString = s.Serialize(values);
                var bytes = GetBytes(json);
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
                var bytes = HttpServerUtility.UrlTokenDecode(base64EncodedSerializedTempData);
                var decompressBytes = Compress.DecompressFromGzip(bytes);
                var dataAsString = GetString(decompressBytes);
                return JsonConvert.DeserializeObject<IDictionary<string, object>>(dataAsString);
            }
            catch
            {
                return null;
            }
        }

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