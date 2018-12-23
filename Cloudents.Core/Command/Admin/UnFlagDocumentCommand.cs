using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Command.Admin
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

