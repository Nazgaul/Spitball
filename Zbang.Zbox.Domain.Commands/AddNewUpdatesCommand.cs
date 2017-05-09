using System;

using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddNewUpdatesCommand : ICommandAsync
    {
        public AddNewUpdatesCommand(long boxId, long userId, Guid? commentId, Guid? replyId, long? itemId, long? quizId, long? itemDiscussionId, long? itemDiscussionReplyId, Guid? quizDiscussionId, long? flashcardId)
        {
            BoxId = boxId;
            UserId = userId;
            CommentId = commentId;
            ReplyId = replyId;
            ItemId = itemId;
            QuizId = quizId;
            ItemDiscussionId = itemDiscussionId;
            ItemDiscussionReplyId = itemDiscussionReplyId;
            QuizDiscussionId = quizDiscussionId;
            FlashcardId = flashcardId;
        }
        public long BoxId { get;private set; }

        public long UserId { get; private set; }

        public Guid? CommentId { get; private set; }
        public Guid? ReplyId { get; private set; }
        public long? ItemId { get; private set; }
        public long? QuizId { get; private set; }

        public long? ItemDiscussionId { get; private set; }
        public long? ItemDiscussionReplyId { get; private set; }
        public Guid? QuizDiscussionId  { get; private set; }

        public long? FlashcardId { get; private set; }


    }
}
