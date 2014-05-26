using System;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CookieHelper
    {
        private HttpContextBase _httpContext;
        public CookieHelper(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            _httpContext = httpContext;
        }

        public void InjectCookie<T>(string cookieName, T cookieData) where T : class
        {
            var cookie = new HttpCookie(cookieName);
            cookie.HttpOnly = true;
            cookie.Value = SerializeData(cookieData);
            _httpContext.Response.Cookies.Add(cookie);
        }
        public T ReadCookie<T>(string cookieName) where T : class
        {
            HttpCookie cookie = _httpContext.Request.Cookies[cookieName];
            if (cookie == null)
            {
                return default(T);
            }
            var obj = Desialize<T>(cookie.Value);

            return obj as T;
        }

        public void RemoveCookie(string cookieName)
        {
            HttpCookie cookie = _httpContext.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1d);
                cookie.Value = String.Empty;
                if (_httpContext.Response != null && _httpContext.Response.Cookies != null)
                {
                    HttpCookie responseCookie = _httpContext.Response.Cookies[cookieName];
                    if (responseCookie != null)
                    {
                        responseCookie.Expires = DateTime.Now.AddDays(-1d);
                        responseCookie.Value = String.Empty;
                    }
                }
            }
        }

        public string SerializeData<T>(T data) where T : class
        {
            var pformatter = new Zbang.Zbox.Infrastructure.Transport.ProtobufSerializer<T>();
            
            var bdata = pformatter.SerializeData(data);
            return HttpServerUtility.UrlTokenEncode(bdata);
            //BinaryFormatter bformatter = new BinaryFormatter();
            // using (var ms = new MemoryStream())
            //{
            //    bformatter.Serialize(ms, data);
            //    return Convert.ToBase64String(ms.ToArray());
            // }
        }

        public object Desialize<T>(string data) where T : class
        {
            var pformatter = new Zbang.Zbox.Infrastructure.Transport.ProtobufSerializer<T>();
            var bytes = HttpServerUtility.UrlTokenDecode(data);
           return pformatter.DeserializeData(bytes);
            //using (var ms = new MemoryStream(bytes))
            //{
            //    BinaryFormatter bformatter = new BinaryFormatter();
            //    return bformatter.Deserialize(ms, null);

            //}
        }
    }
}