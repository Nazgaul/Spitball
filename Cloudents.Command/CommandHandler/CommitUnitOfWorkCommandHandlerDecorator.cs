using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
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
            //try
            // {
            await _decoratee.ExecuteAsync(message, token);
            await _unitOfWork.CommitAsync(token);

            //foreach (var @event in _store)
            //{
            //    await _eventPublisher.PublishAsync(@event, token);
            //}
            
            // }
            //catch (Exception)
            //{
            //   // await _unitOfWork.PublishEventsAsync(token);
            //    throw;
            //}
        }
    }
}