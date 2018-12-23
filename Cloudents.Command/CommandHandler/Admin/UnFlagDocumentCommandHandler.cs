using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UnFlagDocumentCommandHandler : ICommandHandler<UnFlagDocumentCommand>
    {
        private readonly IRepository<Document> _documentRepository;

        public UnFlagDocumentCommandHandler(IRepository<Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }
        public async Task ExecuteAsync(UnFlagDocumentCommand message, CancellationToken token)
        {
            foreach (var id in message.DocumentIds)
            {

            
            var document = await _documentRepository.LoadAsync(id, token);
            document.Item.State = ItemState.Ok;
               
            if (document.Item.FlagReason.Equals("Too many down vote", StringComparison.CurrentCultureIgnoreCase))
            {
                var votes = document.Item.Votes.ToList();
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
}
