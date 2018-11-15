﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core
{
    [UsedImplicitly]
    public sealed class CommandBus : ICommandBus, IDisposable
    {
        public CommandBus(ILifetimeScope container)
        {
            _container = container;
        }

        private readonly ILifetimeScope _container;

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken token) where TCommand : ICommand
        {
            //using (var child = _container.BeginLifetimeScope())
            //{
                var obj = _container.Resolve<ICommandHandler<TCommand>>();
                await obj.ExecuteAsync(command, token).ConfigureAwait(true);
            //}
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}
