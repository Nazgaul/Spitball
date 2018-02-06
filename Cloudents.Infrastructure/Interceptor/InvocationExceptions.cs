using System;
using Castle.DynamicProxy;

namespace Cloudents.Infrastructure.Interceptor
{
    public static class InvocationExceptions
    {
        public static T GetCustomAttribute<T>(this IInvocation invocation) where T : class
        {
            return Attribute.GetCustomAttribute(
                    invocation.MethodInvocationTarget,
                    typeof(T)
                )
                as T;
        }
    }
}
