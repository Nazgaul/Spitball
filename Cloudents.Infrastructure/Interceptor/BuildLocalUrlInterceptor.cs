using System;
using System.Reflection;
using Castle.DynamicProxy;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Interceptor
{
    public class BuildLocalUrlInterceptor : BaseTaskInterceptor<BuildLocalUrlAttribute>
    {
        private readonly IUrlRedirectBuilder _urlRedirectBuilder;

        public BuildLocalUrlInterceptor(IUrlRedirectBuilder urlRedirectBuilder)
        {
            _urlRedirectBuilder = urlRedirectBuilder;
        }

        protected override void BeforeAction(IInvocation invocation)
        {
        }

        protected override void AfterAction<T>(ref T val, IInvocation invocation)
        {

            var att = invocation.GetCustomAttribute<BuildLocalUrlAttribute>();
            var page = 0;
            var pageSize = 0;
            if (att.SizeOfPage.HasValue)
            {
                var pageArg = Array.Find(invocation.Method.GetParameters(), w => w.Name == att.PageArgumentName);
                page = (int)invocation.Arguments[pageArg.Position];
                pageSize = att.SizeOfPage.Value;
            }

            if (string.IsNullOrEmpty(att.ListObjectName) && val != null)
            {
                val = _urlRedirectBuilder.BuildUrl((dynamic)val, page, pageSize);
                return;
            }
            var prop = GetPropertyInfo(val, att.ListObjectName);
            if (prop == null)
            {
                return;
            }
            var data = prop.GetValue(val, null);
            if (data != null)
            {
                var urls = _urlRedirectBuilder.BuildUrl((dynamic)data, page, pageSize);
                prop.SetValue(val, urls);
            }
        }

        private static PropertyInfo GetPropertyInfo(object src, string propName)
        {
            while (true)
            {
                if (src == null) throw new ArgumentException("Value cannot be null.", nameof(src));
                if (propName == null) throw new ArgumentException("Value cannot be null.", nameof(propName));

                if (propName.Contains(".")) //complex type nested
                {
                    var temp = propName.Split(new[] { '.' }, 2);
                    src = GetPropertyInfo(src, temp[0]);
                    propName = temp[1];
                }
                else
                {
                    return src.GetType().GetProperty(propName);
                }
            }
        }
    }
}
