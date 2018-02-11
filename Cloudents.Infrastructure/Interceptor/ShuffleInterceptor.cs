using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Cloudents.Core;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Interceptor
{
    public class ShuffleInterceptor : BaseTaskInterceptor<ShuffleAttribute>
    {
        private readonly IShuffle _shuffle;

        public ShuffleInterceptor(IShuffle shuffle)
        {
            _shuffle = shuffle;
        }

        protected override void BeforeAction(IInvocation invocation)
        {
        }

        protected override void AfterAction<T>(ref T val, IInvocation invocation)
        {
            var att = invocation.GetCustomAttribute<ShuffleAttribute>();
            if (string.IsNullOrEmpty(att.ListObjectName))
            {
                val = _shuffle.DoShuffle((dynamic)val);
                return;
            }
            var prop = GetPropertyInfo(val, att.ListObjectName);
            if (prop == null)
            {
                return;
            }
            var data = prop.GetValue(val, null);
            //if (data.GetType().GetGenericArguments()[0].GetInterfaces().Contains(typeof(IShuffleable)))
            //{
                var urls = _shuffle.DoShuffle((dynamic)data);
                prop.SetValue(val, urls);
            //}
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
