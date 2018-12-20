using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Votes.Commands.AddVoteDocument
{
    public class AddVoteDocumentCommand : ICommand
    {
        public AddVoteDocumentCommand(long userId, long documentId, VoteType voteType)
        {
            UserId = userId;
            DocumentId = documentId;
            VoteType = voteType;
        }

        public long UserId { get; }
        public long DocumentId { get; }

        public VoteType VoteType { get; }
    }
}