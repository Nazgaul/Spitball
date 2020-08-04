using System;
using System.IO;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.EventHandler;
using Cloudents.Core.Storage;

namespace Cloudents.Command.Courses
{
    public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand>
    {
        private readonly IRepository<Tutor> _tutorRepository;
        private readonly IStudyRoomBlobProvider _blobProvider;
        private readonly IGoogleDocument _googleDocument;
        private readonly IDocumentDirectoryBlobProvider _documentBlobProvider;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICourseRepository _courseRepository;

        public CreateCourseCommandHandler(IRepository<Tutor> tutorRepository, IStudyRoomBlobProvider blobProvider, IGoogleDocument googleDocument, IDocumentDirectoryBlobProvider documentBlobProvider, IDocumentRepository documentRepository, ICourseRepository courseRepository)
        {
            _tutorRepository = tutorRepository;
            _blobProvider = blobProvider;
            _googleDocument = googleDocument;
            _documentBlobProvider = documentBlobProvider;
            _documentRepository = documentRepository;
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(CreateCourseCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.UserId, token);

            var studyRooms = message.StudyRooms.ToList();

            if (!studyRooms.Any() && !message.Documents.Any())
            {
                throw new ArgumentException("Cant have empty course");
            }

            var course = new Course(message.Name, tutor, message.Price,
                message.SubscriptionPrice,
                message.Description, studyRooms.DefaultIfEmpty().Min(m=>m?.Date), message.IsPublish);
            tutor.AddCourse(course);


            foreach (var createLiveStudyRoomCommand in studyRooms)
            {
                var documentName = $"{message.Name}-{Guid.NewGuid()}";
                var googleDocUrl = await _googleDocument.CreateOnlineDocAsync(documentName, token);
                var studyRoom = new BroadCastStudyRoom(tutor, googleDocUrl, course, 
                    createLiveStudyRoomCommand.Date, createLiveStudyRoomCommand.Name);
                course.AddStudyRoom(studyRoom);
            }

            foreach (var documentMessage in message.Documents)
            {
                var extension = FileTypesExtensions.FileExtensionsMapping[Path.GetExtension(documentMessage.BlobName)];
                var document = new Document(documentMessage.Name, course, extension.DocumentType, documentMessage.Visible);

                await _documentRepository.AddAsync(document, token);
                var id = document.Id;
                await _documentBlobProvider.MoveAsync(documentMessage.BlobName, id.ToString(), token);
                course.AddDocument(document);

            }
            //need to have this for id generation
            await _courseRepository.AddAsync(course, token);

            if (message.Image != null)
            {
                await _blobProvider.MoveAsync(message.Image, course.Id.ToString(), "0.jpg", token);
            }
            //var user = await _userRepository.LoadAsync(message.UserId, token);
            //user.AssignCourses(new[] { course });
        }
    }
}