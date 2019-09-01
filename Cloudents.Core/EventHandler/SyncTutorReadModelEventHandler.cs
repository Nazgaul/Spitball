using System;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public sealed class SyncTutorReadModelEventHandler : 
        IEventHandler<TutorApprovedEvent>,
        IEventHandler<TutorAddReviewEvent>,
        IEventHandler<UpdateTutorSettingsEvent>,
        IEventHandler<CanTeachCourseEvent>,
        IEventHandler<SetUniversityEvent>,
        IEventHandler<UpdateImageEvent>,
        IEventHandler<EndSessionEvent>,
        IDisposable
    {
        private readonly IReadTutorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SyncTutorReadModelEventHandler(IReadTutorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(TutorApprovedEvent eventMessage, CancellationToken token)
        {
            await AddAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(TutorAddReviewEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(UpdateTutorSettingsEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(CanTeachCourseEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(SetUniversityEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(UpdateImageEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(EndSessionEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.UserId, token);
        }

        private async Task AddAsync(long userId, CancellationToken token)
        {
            var tutor = await _repository.GetReadTutorAsync(userId, token);
            await _repository.AddAsync(tutor, token);
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }

        private async Task UpdateAsync(long userId, CancellationToken token)
        {
            var tutor = await _repository.GetReadTutorAsync(userId, token);
            await _repository.UpdateAsync(tutor, token);
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }

        public void Dispose()
        {
            _repository.Dispose();
            _unitOfWork.Dispose();
        }
    }
}
