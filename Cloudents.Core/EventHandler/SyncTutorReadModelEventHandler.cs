using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public sealed class SyncTutorReadModelEventHandler :
        IEventHandler<TutorApprovedEvent>,
        IEventHandler<TutorAddReviewEvent>,
        IEventHandler<UpdateTutorSettingsEvent>,
        IEventHandler<CanTeachCourseEvent>,
        IEventHandler<RemoveCourseEvent>,
        IEventHandler<SetUniversityEvent>,
        IEventHandler<UpdateImageEvent>,
        IEventHandler<EndStudyRoomSessionEvent>,
        IEventHandler<ChangeCountryEvent>,
        IEventHandler<TutorUnSuspendedEvent>,
        IEventHandler<CourseChangeSubjectEvent>,
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

        public async Task HandleAsync(TutorUnSuspendedEvent eventMessage, CancellationToken token)
        {
            await AddAsync(eventMessage.Id, token);
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
            await UpdateAsync(eventMessage.UserCourse.User.Id, token);
        }

        public async Task HandleAsync(SetUniversityEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(UpdateImageEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(EndStudyRoomSessionEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.Session.StudyRoom.Tutor.Id, token);
        }

        private async Task AddAsync(long userId, CancellationToken token)
        {
            await UpdateAsync(userId, _repository.AddAsync, token);
        }

        private async Task UpdateAsync(long tutorId, Func<ReadTutor, CancellationToken, Task> addOrUpdate, CancellationToken token)
        {
            var tutor = await _repository.GetReadTutorAsync(tutorId, token);
            if (tutor is null)
            {
                return;

            }
            await addOrUpdate(tutor, token);
            await _unitOfWork.CommitAsync(token);
        }

        private async Task UpdateAsync(long userId, CancellationToken token)
        {
            await UpdateAsync(userId, _repository.UpdateAsync, token);
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public async Task HandleAsync(RemoveCourseEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(ChangeCountryEvent eventMessage, CancellationToken token)
        {
            await UpdateAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(CourseChangeSubjectEvent eventMessage, CancellationToken token)
        {
            foreach (var courseUser in eventMessage.Course.Users)
            {
                if (courseUser.User.Tutor != null)
                {
                    await UpdateAsync(courseUser.User.Tutor.Id, token);
                }
            }

        }
    }
}
