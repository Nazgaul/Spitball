﻿using System.Collections.Generic;

namespace Cloudents.Command.Command.Admin
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
