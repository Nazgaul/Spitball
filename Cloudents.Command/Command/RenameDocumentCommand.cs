
namespace Cloudents.Command.Command
{
    public class RenameDocumentCommand : ICommand
    {
        public RenameDocumentCommand(long userId, long documentId, string name)
        {
            UserId = userId;
            DocumentId = documentId;
            Name = name;
        }
        public long UserId { get; set; }
        public long DocumentId { get; set; }
        public string Name { get; set; }
    }
}
