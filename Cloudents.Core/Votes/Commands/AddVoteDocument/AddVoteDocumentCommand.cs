using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Votes.Commands.AddVoteDocument
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