using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command.Admin;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;
using Cloudents.Domain.Enums;

namespace Cloudents.Application.CommandHandler.Admin
{
    class UnFlagDocumentCommandHandler : ICommandHandler<UnFlagDocumentCommand>
    {
        private readonly IRepository<Document> _documentRepository;

        public UnFlagDocumentCommandHandler(IRepository<Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }
        public async Task ExecuteAsync(UnFlagDocumentCommand message, CancellationToken token)
        {
          
            var document = await _documentRepository.LoadAsync(message.DocumentId, token);
            document.Item.State = ItemState.Ok;
               
            if (document.Item.FlagReason.Equals("Too many down vote", StringComparison.CurrentCultureIgnoreCase))
            {
                var votes = document.Item.Votes.ToList<Vote>();
                foreach (var vote in votes)
                {
                document.Item.Votes.Remove(vote);
                }
            }
            document.Item.FlagReason = null;
            document.Item.VoteCount = document.Item.Votes.Count;

            await _documentRepository.UpdateAsync(document, token);

        }
    }
}
