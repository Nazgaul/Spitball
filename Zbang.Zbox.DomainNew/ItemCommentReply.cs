using System;

namespace Zbang.Zbox.Domain
{
    public class ItemCommentReply : ItemCommentBase
    {
        protected ItemCommentReply()
        {

        }
        public ItemCommentReply(User author, Item item, string comment, ItemComment parent, long id)
        {
            if (author == null)
            {
                throw new ArgumentNullException("author");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
           
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
           
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }


// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;//idGenerator.GetId(IdGenerator.ItemAnnotationReplyScope);

            Author = author;
            Item = item;
            Comment = comment;
            Parent = parent;
            UserTime = new UserTimeDetails(Author.Email);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }
        public virtual ItemComment Parent { get; set; }
    }
}