using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Questions.Commands.FlagDocument
{
    public class FlagDocumentCommand : ICommand
    {
        public FlagDocumentCommand(long userId, long documentId, string flagReason)
        {
            UserId = userId;
            DocumentId = documentId;
            FlagReason = flagReason;
        }

        public long UserId { get; }
        public long DocumentId { get; }
        public string FlagReason { get; }
    
    }
}
