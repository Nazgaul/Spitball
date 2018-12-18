using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
{
    public class UnFlagDocumentCommand : ICommand
    {
     
        public UnFlagDocumentCommand(long documentId)
        {
            DocumentId = documentId;
        }



        public long DocumentId { get; private set; }
    }
}

