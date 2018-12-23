using Cloudents.Common.Enum;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Message.System
{
    public class UpdateDocumentNumberOfViews : ISystemQueueMessage
    {

        public UpdateDocumentNumberOfViews(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }

    public class AwardUserWithTokens : ISystemQueueMessage
    {
        public AwardUserWithTokens(long id, decimal amount, TransactionActionType type)
        {
            Id = id;
            Amount = amount;
            Type = type;
        }

        public long Id { get; private set; }

        public decimal Amount { get; set; }

        public TransactionActionType Type { get; set; }
    }
}