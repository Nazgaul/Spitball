using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Command.Courses
{
    public class UpdateCourseCommandHandler : ICommandHandler<UpdateCourseCommand>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IStudyRoomBlobProvider _blobProvider;
        private readonly IGoogleDocument _googleDocument;
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentDirectoryBlobProvider _documentBlobProvider;

        public UpdateCourseCommandHandler(IRepository<Course> courseRepository, IStudyRoomBlobProvider blobProvider, IGoogleDocument googleDocument, IDocumentRepository documentRepository, IDocumentDirectoryBlobProvider documentBlobProvider)
        {
            _courseRepository = courseRepository;
            _blobProvider = blobProvider;
            _googleDocument = googleDocument;
            _documentRepository = documentRepository;
            _documentBlobProvider = documentBlobProvider;
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
            //TODO continue need to check IL users;
            course.State = message.IsPublish ? ItemState.Ok : ItemState.Pending;
            if (message.Image != null)
            {
                await _blobProvider.MoveAsync(message.Image, course.Id.ToString(), "0.jpg", token);
            }



            course.UpdateStudyRoom(message.StudyRooms.Select(s=>new BroadCastStudyRoom(course,s.Date,s.Name)));

            foreach (var broadCastStudyRoom in course.StudyRooms)
            {
                if (broadCastStudyRoom.OnlineDocumentUrl == null)
                {
                    var documentName = $"{message.Name}-{Guid.NewGuid()}";
                    var googleDocUrl = await _googleDocument.CreateOnlineDocAsync(documentName, token);
                    broadCastStudyRoom.OnlineDocumentUrl = googleDocUrl;
                }
            }

            var documentsToUpdate =  message.Documents.Where(w=>w.Id.HasValue).ToDictionary(x => x.Id, x => x);

            foreach (var document in course.Documents)
            {
                if (documentsToUpdate.TryGetValue(document.Id, out var updateStuff))
                {
                    document.Status = updateStuff.Visible ? ItemStatus.Public : ItemStatus.Pending;
                    document.Rename( updateStuff.Name);
                }
                else
                {
                    course.RemoveDocument(document);
                }
            }
            foreach (var newDocuments in message.Documents.Where(w=>!w.Id.HasValue))
            {
                var extension = FileTypesExtensions.FileExtensionsMapping[Path.GetExtension(newDocuments.BlobName)];
                var document = new Document(newDocuments.Name, course, extension.DocumentType, newDocuments.Visible);

                await _documentRepository.AddAsync(document, token);
                var id = document.Id;
                await _documentBlobProvider.MoveAsync(newDocuments.BlobName, id.ToString(), token);
                course.AddDocument(document);
            }
            
          

        }
    }
}