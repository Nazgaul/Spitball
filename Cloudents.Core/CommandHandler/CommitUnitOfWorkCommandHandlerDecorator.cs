using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class CommitUnitOfWorkCommandHandlerDecorator<TCommand>
        : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommandHandler<TCommand> _decoratee;
        private readonly IRepository<Audit> _repository;

        public CommitUnitOfWorkCommandHandlerDecorator(
            IUnitOfWork unitOfWork,
            ICommandHandler<TCommand> decoratee, IRepository<Audit> repository)
        {
            _unitOfWork = unitOfWork;
            _decoratee = decoratee;
            _repository = repository;
        }

        public async Task ExecuteAsync(TCommand message, CancellationToken token)
        {
            await _decoratee.ExecuteAsync(message, token).ConfigureAwait(true);
            var audit = new Audit(message);
            await _repository.AddAsync(audit, token);

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
        private readonly IRepository<Audit> _repository;

        public CommitUnitOfWorkCommandHandlerDecorator(
            IUnitOfWork unitOfWork,
            ICommandHandler<TCommand, TCommandResult> decoratee, IRepository<Audit> repository)
        {
            _unitOfWork = unitOfWork;
            _decoratee = decoratee;
            _repository = repository;
        }

        public async Task<TCommandResult> ExecuteAsync(TCommand command, CancellationToken token)
        {
            var retVal = await _decoratee.ExecuteAsync(command, token).ConfigureAwait(true);
            var audit = new Audit(command);
            await _repository.AddAsync(audit, token);
            await _unitOfWork.CommitAsync(token).ConfigureAwait(true);
            return retVal;
        }
    }
}