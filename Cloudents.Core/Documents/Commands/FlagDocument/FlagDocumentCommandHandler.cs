using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Questions.Commands.FlagDocument;
using Cloudents.Domain.Entities;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.Documents.Commands.FlagDocument
{
    public class FlagDocumentCommandHandler : ICommandHandler<FlagDocumentCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Document> _documentRepository;

        public FlagDocumentCommandHandler(IRepository<RegularUser> userRepository, IRepository<Document> documentRepository)
        {
            _userRepository = userRepository;
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(FlagDocumentCommand message, CancellationToken token)
        {
            if (!ItemComponent.ValidateFlagReason(message.FlagReason))
            {
                throw new ArgumentException("reason is too long");
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);
            if (!Privileges.CanFlag(user.Score))
            {
                throw new UnauthorizedAccessException("not enough score");
            }
            var document = await _documentRepository.LoadAsync(message.DocumentId, token);
            if (document.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot flag your own document");
            }

            document.Item.State = ItemState.Flagged;
            document.Item.FlagReason = message.FlagReason;
            await _documentRepository.UpdateAsync(document, token);
        }
    }
}
