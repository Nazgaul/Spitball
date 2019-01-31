﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Item.Commands.FlagItem
{
    public class FlagDocumentCommandHandler : ICommandHandler<FlagDocumentCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Document> _repository;

        public FlagDocumentCommandHandler(IRepository<RegularUser> userRepository,
            IRepository<Document> documentRepository)
        {
            _userRepository = userRepository;
            _repository = documentRepository;
        }

        public async Task ExecuteAsync(FlagDocumentCommand message, CancellationToken token)
        {
            var document = await _repository.LoadAsync(message.Id, token);
            User user = await _userRepository.LoadAsync(message.UserId, token);


            document.Flag(message.FlagReason, user);
            await _repository.UpdateAsync(document, token);
        }
    }
}
