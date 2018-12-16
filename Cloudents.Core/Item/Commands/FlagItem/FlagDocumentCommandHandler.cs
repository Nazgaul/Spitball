using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Domain.Entities;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.Item.Commands.FlagItem
{
    public class FlagDocumentCommandHandler : BaseFlagItemCommandHandler<Document,long>, ICommandHandler<FlagDocumentCommand>
    {

        public FlagDocumentCommandHandler(IRepository<RegularUser> userRepository,
            IRepository<Document> documentRepository) :base(documentRepository, userRepository)
        {
        }

        public async Task ExecuteAsync(FlagDocumentCommand message, CancellationToken token)
        {
            await base.ExecuteAsync(message, token);
        }

        protected override void Validate(Document document, User user)
        {
            if (document.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot flag your own document");
            }
        }
    }
}
