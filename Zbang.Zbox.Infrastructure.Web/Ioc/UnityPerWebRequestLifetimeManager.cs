using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Practices.Unity;
using System.Collections;

namespace Zbang.Zbox.Infrastructure.Web.Ioc
{
    public class PerHttpRequestLifetime : LifetimeManager
    {
        private readonly Guid _key = Guid.NewGuid();

        Hashtable v = new Hashtable();

        public override object GetValue()
        {
            if (HttpContext.Current == null)
            {
                return v[_key];
            }
            return HttpContext.Current.Items[_key];
        }

        public override void SetValue(object newValue)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[_key] = newValue;
            }
            else
            {
                v[_key] = newValue;
            }
        }

        public override void RemoveValue()
        {
            if (HttpContext.Current != null)
            {
                var obj = GetValue();
                HttpContext.Current.Items.Remove(obj);
            }
            else
            {
                v.Remove(_key);
            }
        }
    }
}
