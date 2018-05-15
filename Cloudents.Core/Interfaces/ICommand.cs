using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ICommand
    {
    }

    public interface ICommandResult
    {
    }

    public interface ICommandHandlerAsync<in TCommand, TCommandResult>
        where TCommand : ICommand
        where TCommandResult : ICommandResult
    {
        Task<TCommandResult> ExecuteAsync(TCommand command, CancellationToken token);
    }

    public interface ICommandHandlerAsync<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand message, CancellationToken token);
    }

    public interface ICommandBus
    {
        Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command, CancellationToken token)
            where TCommand : ICommand
            where TCommandResult : ICommandResult;


        Task DispatchAsync<TCommand>(TCommand command, CancellationToken token)
            where TCommand : ICommand;
    }

}