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
            _unitOfWork = unitOfWork;
            _decoratee = decoratee;
        }

        public async Task ExecuteAsync(TCommand message, CancellationToken token)
        {
            await _decoratee.ExecuteAsync(message, token).ConfigureAwait(true);
            await _unitOfWork.CommitAsync(token).ConfigureAwait(true);
        }
    }
}