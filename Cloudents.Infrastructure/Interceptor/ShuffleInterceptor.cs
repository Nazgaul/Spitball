using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Interceptor
{
    public class ShuffleInterceptor : BaseTaskInterceptor<ShuffleAttribute>
    {
        protected override void BeforeAction(IInvocation invocation)
        {
        }

        protected override void AfterAction<T>(ref T val, IInvocation invocation)
        {
            throw new NotImplementedException();
        }
    }
}
