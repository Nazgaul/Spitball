using System;
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
        private readonly IGoogleDocument _googleDocument;

        public CreateCourseCommandHandler(IRepository<Course> courseRepository, IRepository<Tutor> tutorRepository, IStudyRoomBlobProvider blobProvider, IGoogleDocument googleDocument)
        {
            _courseRepository = courseRepository;
            _tutorRepository = tutorRepository;
            _blobProvider = blobProvider;
            _googleDocument = googleDocument;
        }

        public async Task ExecuteAsync(CreateCourseCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.UserId, token);


            var course = new Course(message.Name, tutor, message.Price,
                message.SubscriptionPrice,
                message.Description);
            tutor.AddCourse(course);


            foreach (var createLiveStudyRoomCommand in message.StudyRooms)
            {
                var documentName = $"{message.Name}-{Guid.NewGuid()}";
                var googleDocUrl = await _googleDocument.CreateOnlineDocAsync(documentName, token);
                var studyRoom = new BroadCastStudyRoom(tutor, googleDocUrl, course, createLiveStudyRoomCommand.Date, createLiveStudyRoomCommand.Name);
                course.AddStudyRoom(studyRoom);
            }
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