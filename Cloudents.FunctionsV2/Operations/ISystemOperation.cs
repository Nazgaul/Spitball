using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.Operations
{
    public interface ISystemOperation<in T> where T : ISystemQueueMessage
    {
        Task DoOperationAsync(T msg, IBinder binder, CancellationToken token);
    }


}