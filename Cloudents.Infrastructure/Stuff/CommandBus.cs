using Autofac;
using Cloudents.Command;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Stuff
{
    public sealed class CommandBus : ICommandBus
    {
        public CommandBus(ILifetimeScope container)
        {
            _container = container;
        }

        private readonly ILifetimeScope _container;

        public async Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command, CancellationToken token) where TCommand : ICommand where TCommandResult : ICommandResult
        {
            using var child = _container.BeginLifetimeScope();
            var obj = child.Resolve<ICommandHandler<TCommand, TCommandResult>>();
            return await obj.ExecuteAsync(command, token);

        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken token) where TCommand : ICommand
        {
            using var child = _container.BeginLifetimeScope();
            var obj = child.Resolve<ICommandHandler<TCommand>>();
            await obj.ExecuteAsync(command, token);
        }
    }
}
