using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Command.Courses
{
    public class UpdateCourseLandingCommandHandler : ICommandHandler<UpdateCourseLandingCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudyRoomBlobProvider _blobProvider;

        public UpdateCourseLandingCommandHandler(ICourseRepository courseRepository, IStudyRoomBlobProvider blobProvider)
        {
            _courseRepository = courseRepository;
            _blobProvider = blobProvider;
        }

        public async Task ExecuteAsync(UpdateCourseLandingCommand message, CancellationToken token)
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

            if (message.HeroSection != null)
            {
                course.Name = message.HeroSection.Name;
                if (message.HeroSection.Image != null)
                {
                    await _blobProvider.MoveAsync(message.HeroSection.Image, course.Id.ToString(), "0.jpg", token);
                }
            }
        }
    }
}