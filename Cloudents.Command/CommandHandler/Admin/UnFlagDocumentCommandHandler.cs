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
        private readonly IRepository<Core.Entities.Document> _documentRepository;

        public UnFlagDocumentCommandHandler(IRepository<Core.Entities.Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }
        public async Task ExecuteAsync(UnFlagDocumentCommand message, CancellationToken token)
        {
            foreach (var id in message.DocumentIds)
            {

            
            var document = await _documentRepository.LoadAsync(id, token);
            document.MakePublic();
               
            if (document.FlagReason.Equals("Too many down vote", StringComparison.CurrentCultureIgnoreCase))
            {
                var votes = document.Votes.ToList();
                foreach (var vote in votes)
                {
                document.Votes.Remove(vote);
                }
            }
            document.VoteCount = document.Votes.Count;

            await _documentRepository.UpdateAsync(document, token);
            }
        }
    }
}
