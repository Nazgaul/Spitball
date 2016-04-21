using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public abstract class Item : IDirty
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
            if (uploader == null) throw new ArgumentNullException(nameof(uploader));
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DateTimeUser = new UserTimeDetails(uploader.Id);

            Name = itemName.Trim();
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
        public virtual long UploaderId { get; set; }
        public virtual long Size { get; set; }
        public virtual Box Box { get; set; }
        public virtual long BoxId { get; set; }
        public virtual int NumberOfViews { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string Url { get; set; }

        public virtual Comment Comment { get; set; }
        public virtual CommentReplies Answer { get; set; }

        protected virtual ICollection<Updates> Updates { get; set; }
        protected virtual ICollection<ItemComment> ItemComments { get; set; }
        protected virtual ICollection<ItemCommentReply> ItemReplies { get; set; }
        public virtual ItemTab Tab { get; protected set; }

        //public virtual IEnumerable<long> GetItemCommentsUserIds()
        //{
        //    return ItemComments.Select(s => s.Author.Id).Union(ItemReplies.Select(s => s.Author.Id));
        //}


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
            var universityName = Box.GetUniversityName() ?? "my";
            Url = UrlConsts.BuildItemUrl(Box.Id, Box.Name, Id, Name, universityName);
        }

        public virtual int LikeCount { get; set; }
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

        public abstract string ChangeName(string newName);

        public ICollection<ItemRate> ItemRates { get; set; }


        public virtual void DeleteAssociation()
        {
            ItemRates.Clear();
            Updates.Clear();
            ItemComments.Clear();
            ItemReplies.Clear();
        }

        public bool IsDirty { get; set; }


        public virtual Func<bool> ShouldMakeDirty { get; set; }
    }
}
