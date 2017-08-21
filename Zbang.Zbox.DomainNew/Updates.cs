using System;

namespace Zbang.Zbox.Domain
{
    public class Updates
    {
        // ReSharper disable DoNotCallOverridableMethodsInConstructor

        protected Updates()
        {

        }

        private Updates(User user, Box box)
        {
            User = user;
            Box = box;
            CreationTime = DateTime.UtcNow;
        }

        public Updates(User user, Box box, Comment comment) : this(user, box)
        {
            Comment = comment;
        }

        public Updates(User user, Box box, Quiz quiz)
            : this(user, box, quiz.Comment)
        {
            Quiz = quiz;
        }

        public Updates(User user, Box box, Item item)
            : this(user, box, item.Comment)
        {
            Item = item;
        }

        public Updates(User user, Box box, CommentReply reply) : this(user, box,reply.Question)
        {
            Reply = reply;
        }

        private Updates(User user, Box box, ItemComment itemComment) : this(user, box, itemComment.Item)
        {
            ItemComment = itemComment;
        }

        private Updates(User user, Box box, ItemCommentReply itemCommentReply) : this(user, box, itemCommentReply.Item)
        {
            ItemCommentReply = itemCommentReply;
        }

        private Updates(User user, Box box, QuizDiscussion quizDiscussion) : this(user, box, quizDiscussion.Quiz)
        {
            QuizDiscussion = quizDiscussion;
        }

        public static Updates UpdateItemDiscussion(User user, Box box, ItemComment itemComment)
        {
            return new Updates(user, box, itemComment);
        }

        public static Updates UpdateItemDiscussionReply(User user, Box box, ItemCommentReply itemReply)
        {
            return new Updates(user, box, itemReply);
        }

        public static Updates UpdateQuizDiscussion(User user, Box box, QuizDiscussion quizDiscussion)
        {
            return new Updates(user, box, quizDiscussion);
        }


        // ReSharper restore DoNotCallOverridableMethodsInConstructor

        public virtual Guid Id { get; set; }
        public virtual User User { get; protected set; }
        public virtual Box Box { get; protected set; }
        public virtual Comment Comment { get; protected set; }
        public virtual CommentReply Reply { get; protected set; }
        public virtual Item Item { get; protected set; }
        public virtual Quiz Quiz { get; protected set; }
        public virtual ItemComment ItemComment { get; protected set; }
        public virtual ItemCommentReply ItemCommentReply { get; protected set; }
        public virtual QuizDiscussion QuizDiscussion { get; protected set; }

        public virtual DateTime CreationTime { get; protected set; }
    }
}
