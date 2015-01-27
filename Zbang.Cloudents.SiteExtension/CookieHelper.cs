using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.SiteExtension
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
                HttpOnly = false,
                Value = value
                //Expires = DateTime.Now.AddDays(1)
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
            if (typeof(T) == typeof(string))
            {
                var x = cookie.Value as T;
                return x;
            }
            var obj2 = DeSerialize<T>(cookie.Value);

            return obj2 as T;
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
            var pFormatter = new Zbox.Infrastructure.Transport.ProtobufSerializer<T>();

            var bData = pFormatter.SerializeData(data);
            return HttpServerUtility.UrlTokenEncode(bData);

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
