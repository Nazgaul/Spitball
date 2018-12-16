using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Item.Commands.FlagItem
{
    public class FlagDocumentCommand : BaseFlagItemCommand<long>
    {
        public FlagDocumentCommand(long userId, long documentId, string flagReason)
            : base(userId, documentId, flagReason)
        {
        }
    }
}
