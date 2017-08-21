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
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }


// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;//idGenerator.GetId(IdGenerator.ItemAnnotationReplyScope);

            Author = author;
            Item = item;
            Comment = comment;
            Parent = parent;
            UserTime = new UserTimeDetails(Author.Id);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public virtual ItemComment Parent { get; set; }
    }
}