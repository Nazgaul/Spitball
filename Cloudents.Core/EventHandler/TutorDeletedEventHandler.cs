using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class TutorDeletedEventHandler : IEventHandler<TutorDeletedEvent>, IEventHandler<TutorSuspendedEvent>, IDisposable
    {
        private readonly IReadTutorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public TutorDeletedEventHandler(IReadTutorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(TutorDeletedEvent eventMessage, CancellationToken token)
        {
            var tutor = await _repository.GetAsync(eventMessage.Id, token);
            if (tutor is null)
            {
                return;
            }
            await _repository.DeleteAsync(tutor, token);
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }

        public async Task HandleAsync(TutorSuspendedEvent eventMessage, CancellationToken token)
        {
            var tutor = await _repository.GetAsync(eventMessage.Id, token);
            if (tutor is null)
            {
                return;
            }
            await _repository.DeleteAsync(tutor, token);
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}
