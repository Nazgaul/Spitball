using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddReplyToAnnotationCommand : ICommand
    {
        public AddReplyToAnnotationCommand(long userId, long itemId, int imageId, string comment, long itemCommentId)
        {
            UserId = userId;
            ItemId = itemId;
            ImageId = imageId;
            Comment = comment;
            ItemCommentId = itemCommentId;
            
        }
        public long UserId { get; private set; }

        public long ItemId { get; private set; }

        public int ImageId { get; private set; }

        public string Comment { get; private set; }

        public long ItemCommentId { get; private set; }

        public long ReplyId { get; set; }
    }
}
