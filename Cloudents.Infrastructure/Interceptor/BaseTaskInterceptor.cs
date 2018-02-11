﻿using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Cloudents.Infrastructure.Interceptor
{
    public abstract class BaseTaskInterceptor<TAtt> : IInterceptor where TAtt : Attribute
    {

        public void Intercept(IInvocation invocation)
        {
            var attr = invocation.GetCustomAttribute<TAtt>();
            if (attr == null)
            {
                invocation.Proceed();
                return;
            }
            BeforeAction(invocation);
            if (invocation.ReturnValue != null)
            {
                return;
            }
            try
            {
                invocation.Proceed();
                if (typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
                {
                    invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex, invocation);
            }
        }

        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            try
            {
                await task;
                //AfterAction<object>(ref null, invocation);
            }
            catch (Exception ex)
            {
                CatchException(ex, invocation);
            }
            // do the logging here, as continuation work for Task...
        }

        private async Task<T> InterceptAsync<T>(Task<T> task, IInvocation invocation)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                AfterAction(ref result, invocation);
                return result;
            }
            catch (Exception ex)
            {
                CatchException(ex, invocation);
                return default;
            }
        }

        public abstract void BeforeAction(IInvocation invocation);
        public abstract void AfterAction<T>(ref T val, IInvocation invocation);

        public virtual void CatchException(Exception ex, IInvocation arg)
        {
            throw ex;
        }
    }
}
