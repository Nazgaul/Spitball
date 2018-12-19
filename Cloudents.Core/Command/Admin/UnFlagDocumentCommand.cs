using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
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

