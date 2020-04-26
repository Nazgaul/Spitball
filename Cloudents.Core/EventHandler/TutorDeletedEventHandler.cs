using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public sealed class TutorDeletedEventHandler :
        IEventHandler<TutorDeletedEvent>,
        IEventHandler<TutorSuspendedEvent>,
        IEventHandler<UserSuspendEvent>,
        IDisposable
    {
        private readonly IReadTutorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public TutorDeletedEventHandler(IReadTutorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public Task HandleAsync(TutorDeletedEvent eventMessage, CancellationToken token)
        {

            return DeleteAsync(eventMessage.Id, token);
        }

        public Task HandleAsync(TutorSuspendedEvent eventMessage, CancellationToken token)
        {

            return DeleteAsync(eventMessage.Id, token);
        }

        public Task HandleAsync(UserSuspendEvent eventMessage, CancellationToken token)
        {

            return DeleteAsync(eventMessage.User.Id, token);
        }

        private async Task DeleteAsync(long tutorId, CancellationToken token)
        {
            var tutor = await _repository.GetAsync(tutorId, token);
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
