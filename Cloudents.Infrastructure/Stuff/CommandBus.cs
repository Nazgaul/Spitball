using Autofac;
using Cloudents.Command;
using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Stuff
{
    [UsedImplicitly]
    public sealed class CommandBus : ICommandBus//, IDisposable
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
                // var unitOfWork = child.Resolve<IUnitOfWork>();
                var obj = child.Resolve<ICommandHandler<TCommand>>();
                await obj.ExecuteAsync(command, token);
                //await unitOfWork.CommitAsync(token);
            }
        }

        //public void Dispose()
        //{
        //    _container?.Dispose();
        //}
    }
}
