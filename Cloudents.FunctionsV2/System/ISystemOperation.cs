using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.System
{
    public interface ISystemOperation<in T> where T : ISystemQueueMessage
    {
        Task DoOperationAsync(T msg, IBinder binder, CancellationToken token);
    }


}