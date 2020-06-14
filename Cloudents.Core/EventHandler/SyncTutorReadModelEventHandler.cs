using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public sealed class SyncTutorReadModelEventHandler :
        IEventHandler<TutorCreatedEvent>,
        IEventHandler<TutorApprovedEvent>,
        IEventHandler<TutorAddReviewEvent>,
        IEventHandler<UpdateTutorSettingsEvent>,
        IEventHandler<CanTeachCourseEvent>,
        IEventHandler<RemoveCourseEvent>,
        IEventHandler<UpdateImageEvent>,
        IEventHandler<EndStudyRoomSessionEvent>,
        IEventHandler<ChangeCountryEvent>,
        IEventHandler<CourseChangeSubjectEvent>,
        IEventHandler<TutorSubscriptionEvent>,
        IEventHandler<TutorSuspendedEvent>,
        IEventHandler<TutorUnSuspendedEvent>,
        IEventHandler<UserChangeNameEvent>,

        IDisposable
    {
        private readonly IReadTutorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SyncTutorReadModelEventHandler(IReadTutorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public Task HandleAsync(TutorApprovedEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.TutorId, token);
        }

        //public Task HandleAsync(TutorUnSuspendedEvent eventMessage, CancellationToken token)
        //{
        //    return SubmitAsync(eventMessage.Id, token);
        //}

        public Task HandleAsync(TutorAddReviewEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.UserId, token);
        }

        public Task HandleAsync(UpdateTutorSettingsEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.UserId, token);
        }

        public Task HandleAsync(CanTeachCourseEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.UserCourse.User.Id, token);
        }

        public Task HandleAsync(UpdateImageEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.UserId, token);
        }

        public Task HandleAsync(EndStudyRoomSessionEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.Session.StudyRoom.Tutor.Id, token);
        }

        //private Task AddAsync(long userId, CancellationToken token)
        //{
        //    return UpdateAsync(userId, _repository.AddAsync, token);
        //}

        private async Task SubmitAsync(long tutorId, CancellationToken token)
        {

            var tutor = await _repository.GetReadTutorAsync(tutorId, token);
            if (tutor is null)
            {
                return;
            }

            await _repository.AddOrUpdateAsync(tutor, token);
            await _unitOfWork.CommitAsync(token);
        }

        //private Task UpdateAsync(long userId, CancellationToken token)
        //{
        //    return SubmitAsync(userId, token);
        //}

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public Task HandleAsync(RemoveCourseEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.UserId, token);
        }

        public Task HandleAsync(ChangeCountryEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.UserId, token);
        }

        public async Task HandleAsync(CourseChangeSubjectEvent eventMessage, CancellationToken token)
        {
            foreach (var courseUser in eventMessage.Course.Users)
            {
                if (courseUser.User.Tutor != null)
                {
                    await SubmitAsync(courseUser.User.Tutor.Id, token);
                }
            }

        }

        public Task HandleAsync(TutorCreatedEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.Tutor.Id, token);
        }

        public Task HandleAsync(TutorSubscriptionEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.UserId, token);
        }

        public Task HandleAsync(TutorSuspendedEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.Id, token);

        }

        public Task HandleAsync(TutorUnSuspendedEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.Id, token);

        }

        public  Task HandleAsync(UserChangeNameEvent eventMessage, CancellationToken token)
        {
            return SubmitAsync(eventMessage.UserId, token);
        }
    }
}
