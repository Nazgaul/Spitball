using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command
{
    public interface ICommand
    {
    }

    //public interface ICommand<T> : ICommand
    //{
    //}

    public interface ICommandResult
    {
    }

    public interface ICommandHandler<in TCommand, TCommandResult>
        where TCommand : ICommand
        where TCommandResult : ICommandResult
    {
        Task<TCommandResult> ExecuteAsync(TCommand command, CancellationToken token);
    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand message, CancellationToken token);
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