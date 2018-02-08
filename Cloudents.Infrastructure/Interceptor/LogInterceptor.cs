using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Interceptor
{
    public class LogInterceptor : BaseTaskInterceptor<LogAttribute>
    {
        private readonly ILogger _logger;

        public LogInterceptor(ILogger logger)
        {
            _logger = logger;
        }


        //private async Task InterceptAsync(IInvocation invocation, Task task)
        //{
        //    try
        //    {
        //        await task;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Exception(ex, new Dictionary<string, string>
        //        {
        //            ["Method"] = invocation.Method.Name,
        //            ["Arg"] = string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())
        //        });
        //    }

        //    // do the logging here, as continuation work for Task...
        //}

        //private async Task<T> InterceptAsync<T>(IInvocation invocation, Task<T> task)
        //{
        //    try
        //    {
        //        var p = await task.ConfigureAwait(false);
        //        return p;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Exception(ex, new Dictionary<string, string>
        //        {
        //            ["Method"] = invocation.Method.Name,
        //            ["Arg"] = string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())
        //        });
        //    }

        //    return default;
        //    // do the logging here, as continuation work for Task<T>...

        //}

        //public void Intercept(IInvocation invocation)
        //{
        //    var attr = invocation.GetCustomAttribute<LogAttribute>();
        //    if (attr == null)
        //    {
        //        invocation.Proceed();
        //        return;
        //    }
        //    try
        //    {
        //        invocation.Proceed();
        //        if (typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
        //        {
        //            invocation.ReturnValue = InterceptAsync(invocation, (dynamic)invocation.ReturnValue);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Exception(ex, new Dictionary<string, string>
        //        {
        //            ["Method"] = invocation.Method.Name,
        //            ["Arg"] = string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())
        //        });
        //        invocation.ReturnValue = default;
        //    }
        //}

        public override void BeforeAction(IInvocation invocation)
        {
        }

        public override void AfterAction<T>(T val, IInvocation invocation)
        {
        }

        public override void CatchException(Exception ex, IInvocation arg)
        {
            _logger.Exception(ex, new Dictionary<string, string>
            {
                ["Method"] = arg.Method.Name,
                ["Arg"] = string.Join(", ", arg.Arguments.Select(a => (a ?? "").ToString()).ToArray())
            });
        }
    }
}