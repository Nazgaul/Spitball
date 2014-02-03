using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public class HttpContextCacheWrapper : IHttpContextCacheWrapper
    {

        public object GetObject(string key)
        {
            if (IsCacheAvaible())
            {
                return HttpContext.Current.Items[key];
            }
            return null;
        }

        public void AddObject(string key, object value)
        {
            if (IsCacheAvaible())
            {
                HttpContext.Current.Items.Add(key, value);
            }
        }

        private bool IsCacheAvaible()
        {
            return HttpContext.Current != null;
        }
    }
}
