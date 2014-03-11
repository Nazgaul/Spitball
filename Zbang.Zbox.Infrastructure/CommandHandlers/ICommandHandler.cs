using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Infrastructure.CommandHandlers
{
    public interface ICommandHandler<TCommand, TCommandResult>
                                    where TCommand: ICommand
                                    where TCommandResult: ICommandResult
    {
        TCommandResult Execute(TCommand command);
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void Handle(TCommand message);
    }

    public interface ICommandHandlerAsync<TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand message);
    }

   
}
