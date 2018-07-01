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

        //public async Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command, CancellationToken token)
        //    where TCommand : ICommand
        //    where TCommandResult : ICommandResult
        //{
        //    using (var child = _container.BeginLifetimeScope())
        //    {
        //        var obj = child.Resolve<ICommandHandler<TCommand, TCommandResult>>();
        //        return await obj.ExecuteAsync(command, token).ConfigureAwait(true);
        //    }
        //}

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken token) where TCommand : ICommand
        {
            using (var child = _container.BeginLifetimeScope())
            {
                var obj = child.Resolve<ICommandHandler<TCommand>>();
                await obj.ExecuteAsync(command, token).ConfigureAwait(true);
            }
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}
