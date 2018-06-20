using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class CommitUnitOfWorkCommandHandlerDecorator<TCommand>
        : ICommandHandler<TCommand> where TCommand : ICommand
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommandHandler<TCommand> _decoratee;

        public CommitUnitOfWorkCommandHandlerDecorator(
            IUnitOfWork unitOfWork,
            ICommandHandler<TCommand> decoratee)
        {
            this._unitOfWork = unitOfWork;
            this._decoratee = decoratee;
        }

        public async Task ExecuteAsync(TCommand command, CancellationToken token)
        {
            await _decoratee.ExecuteAsync(command, token).ConfigureAwait(true);
            await _unitOfWork.CommitAsync(token).ConfigureAwait(true);
        }
    }


    public class CommitUnitOfWorkCommandHandlerDecorator<TCommand, TCommandResult>
        : ICommandHandler<TCommand, TCommandResult>
        where TCommand : ICommand
        where TCommandResult : ICommandResult
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommandHandler<TCommand, TCommandResult> _decoratee;

        public CommitUnitOfWorkCommandHandlerDecorator(
            IUnitOfWork unitOfWork,
            ICommandHandler<TCommand, TCommandResult> decoratee)
        {
            this._unitOfWork = unitOfWork;
            this._decoratee = decoratee;
        }

        public async Task<TCommandResult> ExecuteAsync(TCommand command, CancellationToken token)
        {
            var retVal = await _decoratee.ExecuteAsync(command, token).ConfigureAwait(true);
            await _unitOfWork.CommitAsync(token).ConfigureAwait(true);
            return retVal;
        }
    }
}