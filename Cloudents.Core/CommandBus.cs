using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core
{
    public class CommandBus : ICommandBus
    {
        public CommandBus(ILifetimeScope container)
        {
            _container = container;
        }

        private readonly ILifetimeScope _container;// = IocFactory.Unity;

      

        public async Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command, CancellationToken token)
            where TCommand : ICommand
            where TCommandResult : ICommandResult
        {
            using (var child = _container.BeginLifetimeScope())
            {
                var obj = child.Resolve<ICommandHandlerAsync<TCommand, TCommandResult>>();
                var retVal = await obj.ExecuteAsync(command, token).ConfigureAwait(false);
                return retVal;
            }
        }
    }
}
