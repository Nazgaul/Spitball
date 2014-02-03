
namespace Zbang.Zbox.Domain
{
    public class CommentReplies 
    {
        protected CommentReplies()
        {           
        }

        public CommentReplies(User user, string commentText)
            : this()
        {
            CommentText = commentText;
            Author = user;
            DateTimeUser = new UserTimeDetails(user.Email);
            
        }

        public virtual long Id { get; protected set; }
        public virtual string CommentText { get; private set; }
        public virtual bool IsDeleted { get; set; }
        public virtual User Author { get; private set; }
        public virtual Comment Parent { get; set; }

        public virtual UserTimeDetails DateTimeUser { get; private set; }
    }
}
