using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Cache
{
    public class CacheResultInterceptor : IInterceptor
    {
        private readonly ICacheProvider _cacheProvider;

        public CacheResultInterceptor(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }


        public static CacheAttribute GetCacheResultAttribute(IInvocation invocation)
        {
            return Attribute.GetCustomAttribute(
                    invocation.MethodInvocationTarget,
                    typeof(CacheAttribute)
                )
                as CacheAttribute;
        }

        //private static bool IsSimple(Type type)
        //{
        //    return type.IsPrimitive
        //           || type.IsEnum
        //           || type == typeof(string)
        //           || type == typeof(decimal);
        //}


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
            if (arg is CancellationToken _)
            {
                return string.Empty;
            }
            if (arg is IEnumerable collection)
            {
                var sb = new StringBuilder();
                foreach (var collectionArg in collection)
                {
                    sb.Append(BuildSimpleArgument(collectionArg));
                }

                return sb.ToString();
            }

            return null;
        }

        public static string GetInvocationSignature(IInvocation invocation)
        {
            return
                $"{Assembly.GetExecutingAssembly().GetName().Version.ToString(4)}-" +
                $"{invocation.TargetType.FullName}-{invocation.Method.Name}" +
                $"-{BuildArgument(invocation.Arguments)}";
        }

        private static Task InterceptAsync(string key, CacheAttribute att, Task task)
        {
            return task;
            // do the logging here, as continuation work for Task...
        }

        private async Task<T> InterceptAsync<T>(string key, CacheAttribute att, Task<T> task)
        {
            var result = await task.ConfigureAwait(false);
            if (result != null)
            {
                result = (T)_cacheProvider.Set(key, att.Region, result, att.Duration); // cacheAttr.Duration);
            }
            // do the logging here, as continuation work for Task<T>...
            return result;
        }

        public static Task<T> ConvertAsync<T>(T data)
        {
            return Task.FromResult(data);
        }

        public void Intercept(IInvocation invocation)
        {
            var cacheAttr = GetCacheResultAttribute(invocation);
            if (cacheAttr == null)
            {
                invocation.Proceed();
                return;
            }
            var key = GetInvocationSignature(invocation);

            var method = invocation.MethodInvocationTarget;
            //var isAsync = method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
            var data = _cacheProvider.Get(key, cacheAttr.Region);

            if (data != null)
            {
                if (/*isAsync &&*/ typeof(Task).IsAssignableFrom(method.ReturnType))
                {
                    var taskReturnType = method.ReturnType; //e.g. Task<int>

                    var type = taskReturnType.GetGenericArguments()[0]; //get the result type, e.g. int

                    var convertMethod =
                        GetType().GetMethod(nameof(ConvertAsync))
                            .MakeGenericMethod(type); //Get the closed version of the Convert method, e.g. Convert<int>

                    var result =
                        convertMethod.Invoke(null,
                            new[] { data }); //Call the convert method and return the generic Task, e.g. Task<int>

                    invocation.ReturnValue = result;
                    return;
                }
                invocation.ReturnValue = data;
            }
            invocation.Proceed();

            if (/*isAsync &&*/ typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                invocation.ReturnValue = InterceptAsync(key, cacheAttr, (dynamic)invocation.ReturnValue);
            }
        }
    }
}