using System;
using System.Linq;
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
            course.DomainTime.UpdateTime = DateTime.UtcNow;
            if (message.HeroSection != null)
            {
                course.Name = message.HeroSection.Name;
                if (message.HeroSection.Image != null)
                {
                    await _blobProvider.MoveAsync(message.HeroSection.Image, course.Id.ToString(), "0.jpg", token);
                }

                
            }

            if (message.LiveClassSection != null)
            {
                foreach (var liveClassSection in message.LiveClassSection)
                {
                    var studyRoom = course.StudyRooms.Single(s => s.Id == liveClassSection.Id);
                    studyRoom.Description = liveClassSection.Name;
                }
            }

            course.Details.HeroButton = message.HeroSection?.Button;
            course.Details.ContentText = message.ClassContent?.Text;
            course.Details.ContentTitle = message.ClassContent?.Title;

            course.Details.TeacherBioName = message.TeacherBio?.Name;
            course.Details.TeacherBioText = message.TeacherBio?.Text;
            course.Details.TeacherBioTitle = message.TeacherBio?.Title;
        }
    }
}