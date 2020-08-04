using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Command.Courses
{
    public class UpdateCourseCommandHandler : ICommandHandler<UpdateCourseCommand>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IStudyRoomBlobProvider _blobProvider;

        public UpdateCourseCommandHandler(IRepository<Course> courseRepository, IStudyRoomBlobProvider blobProvider)
        {
            _courseRepository = courseRepository;
            _blobProvider = blobProvider;
        }

        public async Task ExecuteAsync(UpdateCourseCommand message, CancellationToken token)
        {
            var course = await _courseRepository.GetAsync(message.CourseId, token);
            if (course == null)
            {
                throw new NotFoundException();
            }

            if (course.Tutor.Id != message.UserId)
            {
                throw new UnauthorizedAccessException();
            }

            course.Name = message.Name;
            course.Price = course.Price.ChangePrice(message.Price);
            course.Description = message.Description;
            course.ChangeSubscriptionPrice(message.SubscriptionPrice);
            if (message.Image != null)
            {
                await _blobProvider.MoveAsync(message.Image, course.Id.ToString(), "0.jpg", token);
            }

        }
    }
}