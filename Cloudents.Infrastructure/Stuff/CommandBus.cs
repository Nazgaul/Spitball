using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Command;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Stuff
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
            using (var child = _container.BeginLifetimeScope())
            {
                var obj = child.Resolve<ICommandHandler<TCommand>>();
                await obj.ExecuteAsync(command, token);
            }
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}
