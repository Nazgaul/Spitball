﻿using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler
{
    public class CreateDocumentCommandHandler : ICommandHandler<CreateDocumentCommand>
    {
        private readonly IBlobProvider<DocumentContainer> _blobProvider;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<Document> _documentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ITagRepository _tagRepository;

        public CreateDocumentCommandHandler(IBlobProvider<DocumentContainer> blobProvider, IRepository<User> userRepository,
            IRepository<Document> documentRepository, ICourseRepository courseRepository, ITagRepository tagRepository, IRepository<University> universityRepository)
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

            var university = user.University;
            if (message.UniversityId.HasValue)
            {
                university = await _universityRepository.LoadAsync(message.UniversityId.Value, token);
            }

            var itemName = message.Name;
            if (Path.GetExtension(itemName) != Path.GetExtension(message.BlobName))
            {
                itemName += message.BlobName;
            }
            var document = new Document(itemName, /*message.BlobName,*/ university, 
                course, message.Type, tags, user, message.Professor);
            await _documentRepository.AddAsync(document, token).ConfigureAwait(true);
            var id = document.Id;
            await _blobProvider.MoveAsync(message.BlobName, id.ToString(), token);

            message.Id = id;
        }
    }
}