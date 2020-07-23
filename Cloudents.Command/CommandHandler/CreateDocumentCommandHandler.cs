using System;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;

namespace Cloudents.Command.CommandHandler
{
    public class CreateDocumentCommandHandler : ICommandHandler<CreateDocumentCommand>
    {
        private readonly IDocumentDirectoryBlobProvider _blobProvider;
        private readonly IRepository<Tutor> _userRepository;
        private readonly IRepository<Document> _documentRepository;

        public CreateDocumentCommandHandler(IDocumentDirectoryBlobProvider blobProvider,
            IRepository<Tutor> userRepository,
            IRepository<Document> documentRepository
            )
        {
            _blobProvider = blobProvider;
            _userRepository = userRepository;
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(CreateDocumentCommand message, CancellationToken token)
        {
            var tutor = await _userRepository.LoadAsync(message.UserId, token);

            var course = tutor.AddCourse(message.Course);
            var extension = FileTypesExtensions.FileExtensionsMapping[Path.GetExtension(message.BlobName)];

            if (tutor.HasSubscription() && message.Price > 0)
            {
                throw new ArgumentException("tutor with subscription can upload file");
            }
            if (!tutor.HasSubscription() && message.PriceType == PriceType.Subscriber)
            {
                throw new ArgumentException("Only tutor with subscription can upload subscribe plan");   
            }

            var document = new Document(message.Name,
                course, tutor, message.Price.GetValueOrDefault(), extension.DocumentType,
                message.ModelDescription, message.PriceType);

            await _documentRepository.AddAsync(document, token);
            var id = document.Id;
            await _blobProvider.MoveAsync(message.BlobName, id.ToString(), token);
        }
    }
}