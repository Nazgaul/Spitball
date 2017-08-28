using System;
using System.Web;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CookieHelper : ICookieHelper
    {
        private readonly HttpContextBase m_HttpContext;
        public CookieHelper(HttpContextBase httpContext)
        {
            m_HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        public void InjectCookie<T>(string cookieName, T cookieData, bool httpOnly = true) where T : class
        {
            string value;
            if (typeof(T) == typeof(string))
            {
                value = cookieData.ToString();
            }
            else
            {
                value = SerializeData(cookieData);
            }
            var cookie = new HttpCookie(cookieName)
            {
                HttpOnly = httpOnly,
                Value = value
                //Expires = DateTime.Now.AddDays(1)
            };
            m_HttpContext.Response.Cookies.Add(cookie);
        }

        public T ReadCookie<T>(string cookieName) where T : class
        {
            var cookie = m_HttpContext.Request.Cookies[cookieName];
            if (string.IsNullOrEmpty(cookie?.Value))
            {
                return default(T);
            }
            if (typeof(T) == typeof(string))
            {
                var x = cookie.Value as T;
                return x;
            }
            var obj2 = DeSerialize<T>(cookie.Value);

            return obj2 as T;
        }

        public  void RemoveCookie(string cookieName)
        {
            var cookie = m_HttpContext.Request.Cookies[cookieName];
            if (cookie == null) return;
            cookie.Expires = DateTime.Now.AddDays(-1d);
            cookie.Value = string.Empty;
            var responseCookie = m_HttpContext.Response?.Cookies?[cookieName];
            if (responseCookie == null) return;
            responseCookie.Expires = DateTime.Now.AddDays(-1d);
            responseCookie.Value = string.Empty;
        }

        private static string SerializeData<T>(T data) where T : class
        {
            var pFormatter = new Zbox.Infrastructure.Transport.ProtobufSerializer<T>();

            var bData = pFormatter.SerializeData(data);
            return HttpServerUtility.UrlTokenEncode(bData);
        }

        private static object DeSerialize<T>(string data) where T : class
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            try
            {
                var pFormatter = new Zbox.Infrastructure.Transport.ProtobufSerializer<T>();
                var bytes = HttpServerUtility.UrlTokenDecode(data);
                return pFormatter.DeSerializeData(bytes);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On deSerialize data is " + data, ex);
                return null;
            }
        }
    }
}
