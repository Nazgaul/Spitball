using System;
using System.Web;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CookieHelper
    {
        private readonly HttpContextBase m_HttpContext;
        public CookieHelper(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            m_HttpContext = httpContext;
        }

        public void InjectCookie<T>(string cookieName, T cookieData) where T : class
        {
            var cookie = new HttpCookie(cookieName)
            {
                HttpOnly = true,
                Value = SerializeData(cookieData)
            };
            m_HttpContext.Response.Cookies.Add(cookie);
        }
        public T ReadCookie<T>(string cookieName) where T : class
        {
            HttpCookie cookie = m_HttpContext.Request.Cookies[cookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return default(T);
            }
            var obj = DeSerialize<T>(cookie.Value);

            return obj as T;
        }

        public void RemoveCookie(string cookieName)
        {
            HttpCookie cookie = m_HttpContext.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1d);
                cookie.Value = String.Empty;
                if (m_HttpContext.Response != null && m_HttpContext.Response.Cookies != null)
                {
                    HttpCookie responseCookie = m_HttpContext.Response.Cookies[cookieName];
                    if (responseCookie != null)
                    {
                        responseCookie.Expires = DateTime.Now.AddDays(-1d);
                        responseCookie.Value = String.Empty;
                    }
                }
            }
        }

        private string SerializeData<T>(T data) where T : class
        {
            var pformatter = new Zbox.Infrastructure.Transport.ProtobufSerializer<T>();

            var bdata = pformatter.SerializeData(data);
            return HttpServerUtility.UrlTokenEncode(bdata);

        }

        private object DeSerialize<T>(string data) where T : class
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