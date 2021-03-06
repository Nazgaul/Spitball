﻿using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync(IEvent eventMessage) => PublishAsync(eventMessage, default);

        Task PublishAsync(IEvent eventMessage, CancellationToken token);
    }

}