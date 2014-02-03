using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Zbang.Zbox.Infrastructure.Web.Ioc
{
    public class UnityPerWebRequestLifetimeModule : IHttpModule
    {
        //Fields
        private static readonly object m_Key = new object();
        private HttpContextBase m_HttpContext;

        //Ctor
        public UnityPerWebRequestLifetimeModule()
        {
        }

        //For Unit Tests
        public UnityPerWebRequestLifetimeModule(HttpContextBase httpContext)
        {
            this.m_HttpContext = httpContext;
        }

        //Properties
        internal IDictionary<UnityPerWebRequestLifetimeManager, object> Instances
        {
            get
            {
                this.m_HttpContext = (HttpContext.Current != null) ? new HttpContextWrapper(HttpContext.Current) : this.m_HttpContext;

                return (this.m_HttpContext == null) ? null : GetInstances(this.m_HttpContext);
            }
        }

        //Methods
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += (sender, e) => this.RemoveAllInstances();
        }

        internal static IDictionary<UnityPerWebRequestLifetimeManager, object> GetInstances(HttpContextBase httpContext)
        {
            IDictionary<UnityPerWebRequestLifetimeManager, object> instances;

            if (httpContext.Items.Contains(m_Key))
            {
                instances = (IDictionary<UnityPerWebRequestLifetimeManager, object>)httpContext.Items[m_Key];
            }
            else
            {
                lock (httpContext.Items)
                {
                    if (httpContext.Items.Contains(m_Key))
                    {
                        instances = (IDictionary<UnityPerWebRequestLifetimeManager, object>)httpContext.Items[m_Key];
                    }
                    else
                    {
                        instances = new Dictionary<UnityPerWebRequestLifetimeManager, object>();

                        httpContext.Items.Add(m_Key, instances);
                    }
                }
            }

            return instances;
        }

        internal void RemoveAllInstances()
        {
            IDictionary<UnityPerWebRequestLifetimeManager, object> instances = Instances;

            if (instances != null)
            {
                foreach (KeyValuePair<UnityPerWebRequestLifetimeManager, object> entry in instances)
                {
                    IDisposable disposable = entry.Value as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                instances.Clear();
            }
        }
    }
}
