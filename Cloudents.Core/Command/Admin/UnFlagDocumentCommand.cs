using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Command.Admin
{
    public class UnFlagDocumentCommand : ICommand
    {
        public UnFlagDocumentCommand(IEnumerable<long> documentIds)
        {
            DocumentIds = documentIds;
        }
        public UnFlagDocumentCommand(long documentId)
        {
            DocumentIds = new[] { documentId };
        }



        public IEnumerable<long> DocumentIds { get; private set; }
    }
}

