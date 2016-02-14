﻿using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public class ItemComment : ItemCommentBase
    {
        protected ItemComment()
        {

        }
        public ItemComment(User author, Item item, string comment, long id)
        {
            if (author == null)
            {
                throw new ArgumentNullException("author");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }



// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;

            Author = author;
            Item = item;
            Comment = comment.Trim();
            UserTime = new UserTimeDetails(Author.Email);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        protected virtual ICollection<ItemCommentReply> Replies { get; set; }

        public virtual IEnumerable<long> GetUserIdReplies()
        {
            return Replies.Select(s => s.Author.Id);
        }


    }
}
