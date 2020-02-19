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

        public async Task HandleAsync(TutorDeletedEvent eventMessage, CancellationToken token)
        {
            var tutor = await _repository.GetAsync(eventMessage.Id, token);
            if (tutor is null)
            {
                return;
            }
            await DeleteAsync(tutor, token);
        }

        public async Task HandleAsync(TutorSuspendedEvent eventMessage, CancellationToken token)
        {
            var tutor = await _repository.GetAsync(eventMessage.Id, token);
            if (tutor is null)
            {
                return;
            }
            await DeleteAsync(tutor, token);
        }

        public async Task HandleAsync(UserSuspendEvent eventMessage, CancellationToken token)
        {
            var tutor = await _repository.GetAsync(eventMessage.User.Id, token);
            if (tutor is null)
            {
                return;
            }
            await DeleteAsync(tutor, token);
        }

        private async Task DeleteAsync(ReadTutor tutor, CancellationToken token)
        {
            await _repository.DeleteAsync(tutor, token);
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}
