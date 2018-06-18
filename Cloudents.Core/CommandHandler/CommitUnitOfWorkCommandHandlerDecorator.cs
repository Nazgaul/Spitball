using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class CommitUnitOfWorkCommandHandlerDecorator<TCommand>
        : ICommandHandler<TCommand> where TCommand : ICommand
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly ICommandHandler<TCommand> decoratee;

        public CommitUnitOfWorkCommandHandlerDecorator(
            IUnitOfWork unitOfWork,
            ICommandHandler<TCommand> decoratee)
        {
            this.unitOfWork = unitOfWork;
            this.decoratee = decoratee;
        }

        public async Task HandleAsync(TCommand command, CancellationToken token)
        {
            await decoratee.HandleAsync(command, token).ConfigureAwait(false);
            await unitOfWork.CommitAsync(token).ConfigureAwait(false);
        }
    }


    public class CommitUnitOfWorkCommandHandlerDecorator<TCommand, TCommandResult>
        : ICommandHandler<TCommand, TCommandResult>
        where TCommand : ICommand
        where TCommandResult : ICommandResult
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly ICommandHandler<TCommand, TCommandResult> decoratee;

        public CommitUnitOfWorkCommandHandlerDecorator(
            IUnitOfWork unitOfWork,
            ICommandHandler<TCommand, TCommandResult> decoratee)
        {
            this.unitOfWork = unitOfWork;
            this.decoratee = decoratee;
        }

        public async Task<TCommandResult> ExecuteAsync(TCommand command, CancellationToken token)
        {
            var retVal = await decoratee.ExecuteAsync(command, token).ConfigureAwait(false);
            await unitOfWork.CommitAsync(token).ConfigureAwait(false);
            return retVal;
        }
    }
}