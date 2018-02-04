using System;
using System.Collections.Generic;
using System.Linq;

namespace Zbang.Zbox.Domain
{
    public class ItemComment : ItemCommentBase
    {
        // ReSharper disable once UnusedMember.Global nhibernate use
        protected ItemComment()
        {

        }

        public ItemComment(User author, Item item, string comment, long id)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;

            Author = author;
            Item = item;
            Comment = comment.Trim();
            UserTime = new UserTimeDetails(Author.Id);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        protected virtual ICollection<ItemCommentReply> Replies { get; set; }

        public virtual IEnumerable<long> GetUserIdReplies()
        {
            return Replies.Select(s => s.Author.Id);
        }
    }
}
