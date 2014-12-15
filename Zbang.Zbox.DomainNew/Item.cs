using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public abstract class Item : ISoftDeletable
    {
        public const int NameLength = 120;
        protected Item()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            IsDeleted = false;
        }
        protected Item(string itemName, User uploader,
            long iSized, Box box, string itemContentUrl,
            string thumbnailBlobName, string thumbmailUrl)
            : this()
        {
            if (uploader == null) throw new ArgumentNullException("uploader");
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DateTimeUser = new UserTimeDetails(uploader.Email);

            Name = itemName;
            Uploader = uploader;
            Size = iSized;
            Box = box;
            ItemContentUrl = itemContentUrl;
            ThumbnailBlobName = thumbnailBlobName;
            ThumbnailUrl = thumbmailUrl;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

        }
        public virtual long Id { get; protected set; }

        public virtual string Name { get; set; }
        public virtual UserTimeDetails DateTimeUser { get; protected set; }
        public virtual bool IsDeleted { get; set; }

        public virtual User Uploader { get; set; }
        public virtual long Size { get; set; }
        public virtual Box Box { get; set; }
        public virtual int NumberOfViews { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string Url { get; set; }

        public virtual Comment Comment { get; set; }
        public virtual CommentReplies Answer { get; set; }

        protected virtual ICollection<Updates> Updates { get; set; }
        protected virtual ICollection<ItemComment> ItemComments { get; set; }
        protected virtual ICollection<ItemCommentReply> ItemReplies { get; set; }

        public virtual IEnumerable<long> GetItemCommentsUserIds()
        {
            return ItemComments.Select(s => s.Author.Id).Union(ItemReplies.Select(s => s.Author.Id));
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string ItemContentUrl { get; set; }
        public virtual string ThumbnailBlobName { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string ThumbnailUrl { get; private set; }

        public virtual void UpdateThumbnail(string blobName, string url)
        {
            ThumbnailBlobName = blobName;
            ThumbnailUrl = url;
        }

        public virtual void GenerateUrl()
        {
            if (Id == 0)
            {
                return;
            }
            var universityName = Box.Owner.GetUniversityName() ?? "my";
            Url = UrlConsts.BuildItemUrl(Box.Id, Box.Name, Id, Name, universityName);
        }

        public virtual float Rate { get; internal set; }
        public virtual bool Sponsored { get; set; }
        public virtual int NumberOfComments { get; private set; }

        public void IncreaseNumberOfViews()
        {
            NumberOfViews++;
        }

        public virtual void IncreaseNumberOfComments()
        {
            NumberOfComments++;
        }
        public virtual void DecreaseNumberOfComments()
        {
            NumberOfComments--;
        }


        public virtual void CalculateRate(int rate)
        {
            Rate = rate;
        }


        public abstract string ChangeName(string newName);

        public ICollection<ItemRate> ItemRates { get; set; }
    }

    public class Link : Item
    {
        protected Link()
        {

        }
        public Link(string itemName, User uploaderUser, long sized, Box box,
            string linkTitle, string thumbnailBlobName, string thumbnailUrl)
            : base(linkTitle, uploaderUser, sized, box, itemName, thumbnailBlobName, thumbnailUrl)
        { }





        public override string ChangeName(string newName)
        {
            Name = newName;
            GenerateUrl();
            return Name;
        }
    }

    public class File : Item
    {


        public virtual int NumberOfDownloads { get; private set; }

        public virtual string Content { get; set; }


        protected File()
        { }

        public File(string itemName, User uploaderUser, long sized, string blobName,
            string thumbnailBlobName, Box box, string thumbnailUrl)
            : base(itemName, uploaderUser, sized, box, blobName, thumbnailBlobName, thumbnailUrl)
        { }

        public void IncreaseNumberOfDownloads()
        {
            NumberOfDownloads++;
        }



        public override string ChangeName(string newName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newName);
            if (fileNameWithoutExtension == Path.GetFileNameWithoutExtension(Name))
            {
                return Name;
            }


            Name = newName;
            GenerateUrl();
            return Name;
        }
    }
}
