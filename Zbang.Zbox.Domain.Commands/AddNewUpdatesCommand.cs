using System;

using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddNewUpdatesCommand : ICommandAsync
    {
        public AddNewUpdatesCommand(long boxId, long userId, Guid? commentId, Guid? replyId, long? itemId, long? quizId)
        {
            BoxId = boxId;
            UserId = userId;
            CommentId = commentId;
            ReplyId = replyId;
            ItemId = itemId;
            QuizId = quizId;
        }
        public long BoxId { get; set; }

        public long UserId { get; set; }

        public Guid? CommentId { get; set; }
        public Guid? ReplyId { get; set; }
        public long? ItemId { get; set; }
        public long? QuizId { get; set; }
    }
}
