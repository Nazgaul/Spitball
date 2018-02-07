using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Cloudents.Infrastructure.Interceptor
{
    public abstract class BaseTaskInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var needProcess = BeforeAction();
            if (!needProcess)
            {
                return;
            }
            try
            {
                invocation.Proceed();
                if (typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
                {
                    invocation.ReturnValue = InterceptAsync((dynamic) invocation.ReturnValue);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private async Task InterceptAsync(Task task)
        {
            try
            {
                await task;
                AfterAction<object>(null);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            
            // do the logging here, as continuation work for Task...
        }

        private async Task<T> InterceptAsync<T>(Task<T> task)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                AfterAction(result);
                return result;
            }
            catch (Exception ex)
            {
                CatchException(ex);
                return default;
            }
        }

        public abstract bool BeforeAction();
        public abstract void AfterAction<T>(T val);

        public virtual void CatchException(Exception ex)
        {
            throw ex;
        }
    }
}
