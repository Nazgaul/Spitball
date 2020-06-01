using Castle.DynamicProxy;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Infrastructure.Interceptor
{
    public class LogInterceptor : BaseTaskInterceptor<LogAttribute>
    {
        private readonly ILogger _logger;

        public LogInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        protected override void BeforeAction(IInvocation invocation)
        {
        }

        protected override void AfterAction<T>(ref T val, IInvocation invocation)
        {
        }

        protected override void CatchException(Exception ex, IInvocation arg)
        {
            _logger.Exception(ex, new Dictionary<string, string>
            {
                ["Method"] = arg.Method.Name,
                ["Arg"] = string.Join(", ", arg.Arguments.Select(a => (a ?? "").ToString()).ToArray())
            });
        }
    }
}