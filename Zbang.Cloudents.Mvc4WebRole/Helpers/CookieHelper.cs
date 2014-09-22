using System;
using System.Web;

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
            var cookie = new HttpCookie(cookieName) {HttpOnly = true, Value = SerializeData(cookieData)};
            m_HttpContext.Response.Cookies.Add(cookie);
        }
        public T ReadCookie<T>(string cookieName) where T : class
        {
            HttpCookie cookie = m_HttpContext.Request.Cookies[cookieName];
            if (cookie == null)
            {
                return default(T);
            }
            var obj = Desialize<T>(cookie.Value);

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

        private object Desialize<T>(string data) where T : class
        {
            var pformatter = new Zbox.Infrastructure.Transport.ProtobufSerializer<T>();
            var bytes = HttpServerUtility.UrlTokenDecode(data);
           return pformatter.DeSerializeData(bytes);
     
        }
    }
}