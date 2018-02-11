using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Interceptor
{
    public class CacheResultInterceptor : BaseTaskInterceptor<CacheAttribute>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IConfigurationKeys _configurationKeys;

        public CacheResultInterceptor(ICacheProvider cacheProvider, IConfigurationKeys configurationKeys)
        {
            _cacheProvider = cacheProvider;
            _configurationKeys = configurationKeys;
        }

        public static string BuildArgument(object[] argument)
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

        private string GetInvocationSignature(IInvocation invocation)
        {
            return
                $"{_configurationKeys.SystemUrl}" +
                $"{Assembly.GetExecutingAssembly().GetName().Version.ToString(4)}-" +
                $"{invocation.TargetType.FullName}-{invocation.Method.Name}" +
                $"-{BuildArgument(invocation.Arguments)}";
        }

        //private static Task InterceptAsync(string key, CacheAttribute att, Task task)
        //{
        //    return task;
        //    // do the logging here, as continuation work for Task...
        //}

        //private async Task<T> InterceptAsync<T>(string key, CacheAttribute att, Task<T> task)
        //{
        //    var result = await task.ConfigureAwait(false);
        //    if (result != null)
        //    {
        //        result = (T)_cacheProvider.Set(key, att.Region, result, att.Duration); // cacheAttr.Duration);
        //    }
        //    // do the logging here, as continuation work for Task<T>...
        //    return result;
        //}

        private static Task<T> ConvertAsync<T>(T data)
        {
            return Task.FromResult(data);
        }

        //public void Intercept(IInvocation invocation)
        //{
        //    var cacheAttr = invocation.GetCustomAttribute<CacheAttribute>();
        //    if (cacheAttr == null)
        //    {
        //        invocation.Proceed();
        //        return;
        //    }
        //    var key = GetInvocationSignature(invocation);

        //    var method = invocation.MethodInvocationTarget;
        //    var data = _cacheProvider.Get(key, cacheAttr.Region);

        //    if (data != null)
        //    {
        //        if (typeof(Task).IsAssignableFrom(method.ReturnType))
        //        {
        //            var taskReturnType = method.ReturnType; //e.g. Task<int>

        //            var type = taskReturnType.GetGenericArguments()[0]; //get the result type, e.g. int

        //            var convertMethod =
        //                GetType().GetMethod(nameof(ConvertAsync), BindingFlags.Static | BindingFlags.NonPublic)
        //                    .MakeGenericMethod(type); //Get the closed version of the Convert method, e.g. Convert<int>

        //            var result =
        //                convertMethod.Invoke(null,
        //                    new[] { data }); //Call the convert method and return the generic Task, e.g. Task<int>

        //            invocation.ReturnValue = result;
        //            return;
        //        }
        //        invocation.ReturnValue = data;
        //        return;
        //    }
        //    invocation.Proceed();

        //    if (typeof(Task).IsAssignableFrom(method.ReturnType))
        //    {
        //        invocation.ReturnValue = InterceptAsync(key, cacheAttr, (dynamic)invocation.ReturnValue);
        //    }
        //}

        protected override void BeforeAction(IInvocation invocation)
        {
            var key = GetInvocationSignature(invocation);

            var method = invocation.MethodInvocationTarget;
            var att = invocation.GetCustomAttribute<CacheAttribute>();
            var data = _cacheProvider.Get(key, att.Region);

            if (data != null)
            {
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
        }

        protected override void AfterAction<T>(ref T val, IInvocation invocation)
        {
            var key = GetInvocationSignature(invocation);
            var att = invocation.GetCustomAttribute<CacheAttribute>();
            val = (T)_cacheProvider.Set(key, att.Region, val, att.Duration); // cacheAttr.Duration);
        }
    }
}