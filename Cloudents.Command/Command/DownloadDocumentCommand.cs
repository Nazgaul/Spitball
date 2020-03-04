
namespace Cloudents.Command.Command
{
    public class DownloadDocumentCommand : ICommand
    {
        public DownloadDocumentCommand(long documentId, long userId)
        {
            DocumentId = documentId;
            UserId = userId;
        }
        public long DocumentId { get;  }
        public long UserId { get;  }
    }
}
