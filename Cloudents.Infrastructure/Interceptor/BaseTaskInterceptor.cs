using System;
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
                    var x = InterceptAsync((dynamic)invocation.ReturnValue, invocation);
                    
                    invocation.ReturnValue = x;// InterceptAsync((dynamic)invocation.ReturnValue, invocation);
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
                var result = await task;
                if (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(result, default))
                {
                    AfterAction(ref result, invocation);
                }

                return result;
            }
            catch (Exception ex)
            {
                CatchException(ex, invocation);
                return default;
            }
        }

        protected abstract void BeforeAction(IInvocation invocation);

        /// <summary>
        /// After invocation action
        /// </summary>
        /// <typeparam name="T">The object</typeparam>
        /// <param name="val">the value - ref because we change the object</param>
        /// <param name="invocation"></param>
        protected abstract void AfterAction<T>(ref T val, IInvocation invocation);

        protected virtual void CatchException(Exception ex, IInvocation arg)
        {
            throw ex;
        }
    }
}
