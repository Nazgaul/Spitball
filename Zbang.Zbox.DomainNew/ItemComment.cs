
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public abstract class ItemCommentBase
    {
        public virtual long Id { get; set; }
        public virtual Item Item { get; set; }
        public virtual int ImageId { get; set; }

        public virtual string Comment { get; set; }

        public virtual UserTimeDetails UserTime { get; set; }

        public virtual User Author { get; set; }
    }
    public class ItemComment : ItemCommentBase
    {
        protected ItemComment()
        {

        }
        public ItemComment(User author, Item item, int imageId, string comment, int cordX, int cordY, int width, int height)
        {
            Throw.OnNull(author, "author");
            Throw.OnNull(item, "item");
            Throw.OnNegative(imageId, "imageId");
            Throw.OnNegative(cordX, "cordX");
            Throw.OnNegative(cordY, "cordY");
            Throw.OnNegative(width, "width");
            Throw.OnNegative(height, "height");
            Throw.OnNull(comment, "comment", false);


            var idGenerator = Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();

// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = idGenerator.GetId(IdGenerator.ItemAnnotationScope);

            Author = author;
            Item = item;
            ImageId = imageId;
            Comment = comment.Trim();
            CordX = cordX;
            CordY = cordY;
            Width = width;
            Height = height;
            UserTime = new UserTimeDetails(Author.Email);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }
        public virtual int CordX { get; set; }
        public virtual int CordY { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }


    }

    public class ItemCommentReply : ItemCommentBase
    {
        protected ItemCommentReply()
        {

        }
        public ItemCommentReply(User author, Item item, int imageId, string comment, ItemComment parent)
        {
            Throw.OnNull(author, "author");
            Throw.OnNull(item, "item");
            Throw.OnNegative(imageId, "imageId");
            Throw.OnNull(comment, "comment", false);
            Throw.OnNull(parent, "parent");


            var idGenerator = Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = idGenerator.GetId(IdGenerator.ItemAnnotationReplyScope);

            Author = author;
            Item = item;
            ImageId = imageId;
            Comment = comment;
            Parent = parent;
            UserTime = new UserTimeDetails(Author.Email);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }
        public virtual ItemComment Parent { get; set; }
    }
}
