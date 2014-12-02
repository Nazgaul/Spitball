using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Infrastructure.CommandHandlers
{
    public interface ICommandHandler<in TCommand, out TCommandResult>
        where TCommand : ICommand
        where TCommandResult : ICommandResult
    {
        TCommandResult Execute(TCommand command);
    }


    public interface ICommandHandlerAsync<in TCommand, TCommandResult>
        where TCommand : ICommand
        where TCommandResult : ICommandResult
    {
        Task<TCommandResult> ExecuteAsync(TCommand command);
    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand message);
    }

    public interface ICommandHandlerAsync<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand message);
    }


}
