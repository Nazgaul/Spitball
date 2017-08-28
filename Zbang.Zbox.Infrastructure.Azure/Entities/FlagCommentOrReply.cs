using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Zbang.Zbox.Infrastructure.Azure.Entities
{
    public class FlagCommentOrReply : TableEntity
    {
        public FlagCommentOrReply(Guid postId, long userId )
            : base("FlagPost", postId.ToString())
        {
            UserId = userId;
            PostId = postId;
        }

        public FlagCommentOrReply()
        {
        }

        public long UserId { get; set; }

        public Guid PostId { get; set; }
    }
}