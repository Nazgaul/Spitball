using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Command.CommandHandler
{
    public class CreateDocumentCommandHandler : ICommandHandler<CreateDocumentCommand>
    {
        private readonly IDocumentDirectoryBlobProvider _blobProvider;
        private readonly IRepository<BaseUser> _userRepository;
        private readonly IRepository<Document> _documentRepository;
        private readonly IRepository<Course> _courseRepository;

        public CreateDocumentCommandHandler(IDocumentDirectoryBlobProvider blobProvider,
            IRepository<BaseUser> userRepository,
            IRepository<Document> documentRepository, IRepository<Course> courseRepository
            )
        {
            _blobProvider = blobProvider;
            _userRepository = userRepository;
            _documentRepository = documentRepository;
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(CreateDocumentCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var course = await _courseRepository.LoadAsync(message.Course, token);
            var extension = FileTypesExtensions.FileExtensionsMapping[Path.GetExtension(message.BlobName)];
            var document = new Document(Path.GetFileNameWithoutExtension(message.Name), 
                course,  user,  message.Price, extension.DocumentType,message.ModelDescription);
     
            await _documentRepository.AddAsync(document, token);
            var id = document.Id;
            await _blobProvider.MoveAsync(message.BlobName, id.ToString(), token);
        }
    }
}