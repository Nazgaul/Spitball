using Microsoft.Practices.Unity;
using System;
using System.Collections;

using System.Web;

namespace Zbang.Zbox.Infrastructure.Ioc
{
    internal class PerHttpRequestLifetime : LifetimeManager
    {
        private string _key = string.Format("PerCallContextOrRequestLifeTimeManager_{0}", Guid.NewGuid());
        //private readonly Guid _key = Guid.NewGuid();
        //private const string _key = "SingletonPerCallContext";

        private bool IsOnWeb()
        {
            return HttpContext.Current != null;
        }

        readonly Hashtable v = new Hashtable();

        public override object GetValue()
        {
            if (!IsOnWeb())
            {
                return v[_key];
            }
            return HttpContext.Current.Items[_key];
        }

        public override void SetValue(object newValue)
        {
            if (IsOnWeb())
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
            if (IsOnWeb())
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
