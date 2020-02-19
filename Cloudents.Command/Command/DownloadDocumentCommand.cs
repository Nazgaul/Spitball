
namespace Cloudents.Command.Command
{
    public class DownloadDocumentCommand : ICommand
    {
        public DownloadDocumentCommand(long documentId, long userId)
        {
            DocumentId = documentId;
            UserId = userId;
        }
        public long DocumentId { get; set; }
        public long UserId { get; set; }
    }
}
