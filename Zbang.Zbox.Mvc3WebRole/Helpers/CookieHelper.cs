using System;
using System.IO;
using System.Linq;
using System.Web;

namespace Zbang.Zbox.Mvc3WebRole.Helpers
{
    public class CookieHelper
    {
        HttpContextBase _httpContext;

        public CookieHelper(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            _httpContext = httpContext;
        }

        public void SaveData(object value)
        {
            string cookieValue = ObjectToBase64String(value);

            var cookie = new HttpCookie(value.GetType().Name);
            cookie.HttpOnly = true;
            cookie.Value = cookieValue;

            _httpContext.Response.Cookies.Add(cookie);
        }

        public T ReadData<T>() where T : class, new()
        {
            var cookieName = typeof(T).Name;
            HttpCookie cookie = _httpContext.Request.Cookies[cookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value)) {
                T deserializedDictionary = Base64StringToObject<T>(cookie.Value);

                //cookie.Expires = DateTime.MinValue;
                //cookie.Value = string.Empty;

                //if (_httpContext.Response != null && _httpContext.Response.Cookies != null) {
                //    HttpCookie responseCookie = _httpContext.Response.Cookies[TempDataCookieKey];
                //    if (responseCookie != null) {
                //        cookie.Expires = DateTime.MinValue;
                //        cookie.Value = string.Empty;
                //    }
                //}

                return deserializedDictionary;
            }

            return null;
        }

        private T Base64StringToObject<T>(string p)
        {
            SetupRuntimeSerialazation(typeof(T));

            byte[] bytes = Convert.FromBase64String(p);
            using (var memStream = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize<T>(memStream);
            }
        }

        private void SetupRuntimeSerialazation(Type t)
        {
            var m = ProtoBuf.Meta.RuntimeTypeModel.Default;
            if (m.CanSerialize(t))
            {
                return;
            }
            
             var properties = t.GetProperties().Select(p => p.Name).OrderBy(name => name);//OrderBy added, thanks MG
             m.Add(t, true).Add(properties.ToArray());   
        }

        private string ObjectToBase64String(object value)
        {
            SetupRuntimeSerialazation(value.GetType());

            
            //using (var file = File.OpenRead("person.bin"))
            //{
            //    newPerson = Serializer.Deserialize<Person>(file);
            //}
            using (MemoryStream memStream = new MemoryStream())
            {
                memStream.Seek(0, SeekOrigin.Begin);

                ProtoBuf.Serializer.Serialize(memStream,value);
                memStream.Seek(0, SeekOrigin.Begin);
                byte[] bytes = memStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
    }
}