using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler
{
    public class CreateDocumentCommandHandler : ICommandHandler<CreateDocumentCommand>
    {
        private readonly IBlobProvider<DocumentContainer> _blobProvider;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Document> _documentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ITagRepository _tagRepository;

        public CreateDocumentCommandHandler(IBlobProvider<DocumentContainer> blobProvider, IRepository<User> userRepository,
            IRepository<Document> documentRepository, ICourseRepository courseRepository, ITagRepository tagRepository)
        {
            _blobProvider = blobProvider;
            _userRepository = userRepository;
            _documentRepository = documentRepository;
            _courseRepository = courseRepository;
            _tagRepository = tagRepository;
        }

        public async Task ExecuteAsync(CreateDocumentCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            var course = await _courseRepository.GetOrAddAsync(message.Course, token);
            var tags = new List<Tag>();

            if (message.Tags != null)
            {
                foreach (var tagName in message.Tags)
                {
                    var tag = await _tagRepository.GetOrAddAsync(tagName, token);
                    tags.Add(tag);
                }
            }

            var document = new Document(message.Name, message.BlobName, user.University, 
                course, message.Type, tags, user, message.Professor);
            await _documentRepository.AddAsync(document, token).ConfigureAwait(true);
            var id = document.Id;
            await _blobProvider.MoveAsync(message.BlobName, id.ToString(), token);

            message.Id = id;
        }
    }
}