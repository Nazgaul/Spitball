using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;

namespace Cloudents.Command.Courses
{
    public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Tutor> _tutorRepository;
        //private readonly IRepository<BroadCastStudyRoom> _studyRoomRepository;
        private readonly IStudyRoomBlobProvider _blobProvider;

        public CreateCourseCommandHandler(IRepository<Course> courseRepository, IRepository<Tutor> tutorRepository, IStudyRoomBlobProvider blobProvider)
        {
            _courseRepository = courseRepository;
            _tutorRepository = tutorRepository;
            _blobProvider = blobProvider;
        }

        public async Task ExecuteAsync(CreateCourseCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.UserId, token);


            var course = new Course(message.Name, tutor, message.Price,
                message.SubscriptionPrice,
                message.Description);
            tutor.AddCourse(course);

           //await _courseRepository.AddAsync(course, token);

            if (message.Image != null)
            {
                await _blobProvider.MoveAsync(message.Image, course.Id.ToString(), "0.jpg", token);
            }
            //var user = await _userRepository.LoadAsync(message.UserId, token);
            //user.AssignCourses(new[] { course });
        }
    }
}