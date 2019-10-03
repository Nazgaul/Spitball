using System.Collections.Generic;
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
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<Document> _documentRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly ITagRepository _tagRepository;

        public CreateDocumentCommandHandler(IDocumentDirectoryBlobProvider blobProvider,
            IRepository<BaseUser> userRepository,
            IRepository<Document> documentRepository, IRepository<Course> courseRepository,
            ITagRepository tagRepository, IRepository<University> universityRepository)
        {
            _blobProvider = blobProvider;
            _userRepository = userRepository;
            _documentRepository = documentRepository;
            _courseRepository = courseRepository;
            _tagRepository = tagRepository;
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(CreateDocumentCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            var course = await _courseRepository.LoadAsync(message.Course, token);

            var tags = new List<Tag>();

            if (message.Tags != null)
            {
                foreach (var tagName in message.Tags)
                {
                    var tag = await _tagRepository.GetOrAddAsync(tagName, token);
                    tags.Add(tag);
                }
            }

            var university = user.University;
            if (message.UniversityId.HasValue)
            {
                university = await _universityRepository.LoadAsync(message.UniversityId.Value, token);
            }

            var extension = FileTypesExtensions.FileExtensionsMapping[Path.GetExtension(message.BlobName)];
            

            var document = new Document(Path.GetFileNameWithoutExtension(message.Name), university,
                course, message.Type, tags, user, message.Professor, message.Price, extension.DocumentType);
            await _documentRepository.AddAsync(document, token);
            var id = document.Id;
            await _blobProvider.MoveAsync(message.BlobName, id.ToString(), token);

            message.Id = id;
        }
    }
}