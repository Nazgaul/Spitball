using System;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public abstract class ItemCommentBase
    {
        public virtual long Id { get; set; }
        public virtual Item Item { get; set; }
        //public virtual int ImageId { get; set; }

        public virtual string Comment { get; set; }

        public virtual UserTimeDetails UserTime { get; set; }

        public virtual User Author { get; set; }
    }
    public class ItemComment : ItemCommentBase
    {
        protected ItemComment()
        {

        }
        public ItemComment(User author, Item item, string comment)
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


            var idGenerator = Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();

// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = idGenerator.GetId(IdGenerator.ItemAnnotationScope);

            Author = author;
            Item = item;
            Comment = comment.Trim();
            UserTime = new UserTimeDetails(Author.Email);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }




    }

    public class ItemCommentReply : ItemCommentBase
    {
        protected ItemCommentReply()
        {

        }
        public ItemCommentReply(User author, Item item, string comment, ItemComment parent)
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


            var idGenerator = Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = idGenerator.GetId(IdGenerator.ItemAnnotationReplyScope);

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
