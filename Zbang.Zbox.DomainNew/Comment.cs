using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class Comment : CommentReplies
    {
        protected Comment()
        {
            Replies = new List<CommentReplies>();
        }
        public Comment(User user, string commentText, Box box, Item item)
            : base(user, commentText)
        {
            Box = box;
            Item = item;
           
        }
      
        protected virtual ICollection<CommentReplies> Replies { get; set; }
        protected virtual Box Box { get; set; }
        protected virtual Item Item { get; set; }


        public CommentReplies AddComment(User author, string commentText)
        {
            var comment = new CommentReplies(author, commentText) {Parent = this};
            Replies.Add(comment);

            DateTimeUser.UpdateUserTime(author.Email);
            return comment;
        }

        public void DeleteReplies(string author)
        {
            foreach (var reply in Replies)
            {
                reply.IsDeleted = true;
                reply.DateTimeUser.UpdateUserTime(author);
            }
            Box.UserTime.UpdateUserTime(author);
            if (Item != null)
            {
                Item.DateTimeUser.UpdateUserTime(author);
            }
        }

    }
}
