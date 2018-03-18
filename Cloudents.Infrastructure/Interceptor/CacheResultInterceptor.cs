using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Interceptor
{
    public class CacheResultInterceptor : BaseTaskInterceptor<CacheAttribute>
    {
        private readonly ICacheProvider _cacheProvider;

        public CacheResultInterceptor(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        private static string BuildArgument(IEnumerable<object> argument)
        {
            var sb = new StringBuilder();
            foreach (var arg in argument)
            {
                var result = BuildSimpleArgument(arg);
                if (result == null)
                {
                    foreach (var prop in arg.GetType().GetProperties())
                    {
                        sb.Append(prop.Name).Append("=").Append(BuildSimpleArgument(prop.GetValue(arg)) ?? string.Empty);
                    }
                }
                else
                {
                    sb.Append(result);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Build simple argument
        /// </summary>
        /// <param name="arg">the object</param>
        /// <returns>string or empty if can process null if cant</returns>
        private static string BuildSimpleArgument(object arg)
        {
            if (arg == null)
            {
                return string.Empty;
            }
            if (arg.ToString() != arg.GetType().ToString())
            {
                return arg.ToString();
                // This Type or one of its base types has overridden object.ToString()
            }
            switch (arg)
            {
                case CancellationToken _:
                    return string.Empty;
                case IEnumerable collection:
                    var list = new List<string>();
                    foreach (var collectionArg in collection)
                    {
                        list.Add(BuildSimpleArgument(collectionArg));
                    }

                    list.Sort();
                    return string.Concat(list);// sb.ToString();
            }

            return null;
        }

        private static string GetInvocationSignature(IInvocation invocation)
        {
#if DEBUG
            const string prefix = "Debug";
#else
            const string  prefix = "Release";
#endif
            return
                $"{prefix}-{Assembly.GetExecutingAssembly().GetName().Version.ToString(4)}-" +
                $"{invocation.TargetType.FullName}-{invocation.Method.Name}" +
                $"-{BuildArgument(invocation.Arguments)}";
        }

        [UsedImplicitly]
        private static Task<T> ConvertAsync<T>(T data)
        {
            return Task.FromResult(data);
        }

        protected override void BeforeAction(IInvocation invocation)
        {
            var key = GetInvocationSignature(invocation);

            var method = invocation.MethodInvocationTarget;
            var att = invocation.GetCustomAttribute<CacheAttribute>();
            var data = _cacheProvider.Get(key, att.Region);

            if (data == null) return;
            if (typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                var taskReturnType = method.ReturnType; //e.g. Task<int>

                var type = taskReturnType.GetGenericArguments()[0]; //get the result type, e.g. int

                var convertMethod =
                    GetType().GetMethod(nameof(ConvertAsync), BindingFlags.Static | BindingFlags.NonPublic)
                        .MakeGenericMethod(type); //Get the closed version of the Convert method, e.g. Convert<int>

                invocation.ReturnValue =
                    convertMethod.Invoke(null,
                        new[] { data }); //Call the convert method and return the generic Task, e.g. Task<int>

                return;
            }
            invocation.ReturnValue = data;
        }

        protected override void AfterAction<T>(ref T val, IInvocation invocation)
        {
            var key = GetInvocationSignature(invocation);
            var att = invocation.GetCustomAttribute<CacheAttribute>();
            _cacheProvider.Set(key, att.Region, val, att.Duration,att.Slide); // cacheAttr.Duration);
        }
    }
}