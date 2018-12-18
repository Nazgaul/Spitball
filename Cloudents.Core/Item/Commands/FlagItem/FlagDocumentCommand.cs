﻿namespace Cloudents.Core.Item.Commands.FlagItem
{
    public class FlagDocumentCommand : BaseFlagItemCommand<long>
    {
        public FlagDocumentCommand(long userId, long documentId, string flagReason)
        {
            Id = documentId;
            FlagReason = flagReason;
            UserId = userId;
        }

        internal FlagDocumentCommand(long answerId)
        {
            Id = answerId;
            FlagReason = "Too many down vote";
        }
    }
}
