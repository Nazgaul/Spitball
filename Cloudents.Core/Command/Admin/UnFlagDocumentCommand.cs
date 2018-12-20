using System.Collections.Generic;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
{
    public class UnFlagDocumentCommand : ICommand
    {
     
        public UnFlagDocumentCommand(long documentId)
        {
            DocumentIds = new[] { documentId };
        }

        public UnFlagDocumentCommand(IEnumerable<long> documentIds)
        {
            DocumentIds = documentIds;
        }

        public IEnumerable<long> DocumentIds { get; private set; }
    }
}

