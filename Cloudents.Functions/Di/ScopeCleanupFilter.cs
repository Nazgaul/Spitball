﻿using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions.Di
{
    public class ScopeCleanupFilter : IFunctionInvocationFilter, IFunctionExceptionFilter
    {
        public Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken)
        {
            RemoveScope(exceptionContext.FunctionInstanceId);
            return Task.CompletedTask;
        }

        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            RemoveScope(executedContext.FunctionInstanceId);
            return Task.CompletedTask;
        }

        public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        private static void RemoveScope(Guid id)
        {
            if (InjectBindingProvider.Scopes.TryRemove(id, out var scope))
            {
                scope.Dispose();
            }
        }
    }
}
