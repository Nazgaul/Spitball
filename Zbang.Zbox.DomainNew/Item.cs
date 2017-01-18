using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
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
            long iSized, Box box, string itemContentUrl)
            : this()
        {
            if (uploader == null) throw new ArgumentNullException(nameof(uploader));
            if (string.IsNullOrEmpty(itemName)) throw new ArgumentNullException(nameof(itemName));
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DateTimeUser = new UserTimeDetails(uploader.Id);

            Name = itemName.Trim();
            User = uploader;
            Size = iSized;
            Box = box;
            ItemContentUrl = itemContentUrl;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

        }
        public virtual long Id { get; protected set; }

        public virtual string Name { get; set; }
        public virtual UserTimeDetails DateTimeUser { get; protected set; }
        public virtual bool IsDeleted { get; set; }

        public virtual User User { get; set; }
        public virtual long UploaderId { get; set; }
        public virtual long Size { get; set; }
        public virtual Box Box { get; set; }
        public virtual long BoxId { get; set; }
        public virtual int NumberOfViews { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string Url { get; set; }

        public virtual Comment Comment { get; set; }
        public virtual CommentReply CommentReply { get; set; }

        protected virtual ICollection<Updates> Updates { get; set; }
        protected virtual ICollection<ItemComment> ItemComments { get; set; }
        protected virtual ICollection<ItemCommentReply> ItemReplies { get; set; }
        public virtual ItemTab Tab { get; protected set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string ItemContentUrl { get; set; }

        public virtual void GenerateUrl()
        {
            if (Id == 0)
            {
                return;
            }
            var universityName = Box.GetUniversityName() ?? "my";
            Url = UrlConst.BuildItemUrl(Box.Id, Box.Name, Id, Name, universityName);
            ShouldMakeDirty = () => true;
            //IsDirty = true;
        }

        public virtual int LikeCount { get; set; }
        //public virtual bool Sponsored { get; set; }
        //public virtual int NumberOfComments { get; private set; }

        public void IncreaseNumberOfViews()
        {
            NumberOfViews++;
        }

        //public virtual void IncreaseNumberOfComments()
        //{
        //    NumberOfComments++;
        //}
        //public virtual void DecreaseNumberOfComments()
        //{
        //    NumberOfComments--;
        //}

        public abstract string ChangeName(string newName);

        public ICollection<ItemRate> ItemRates { get; set; }


        public virtual void DeleteAssociation()
        {
            ItemRates.Clear();
            Updates.Clear();
            ItemComments.Clear();
            ItemReplies.Clear();
        }

        public DirtyState IsDirty { get; set; }


        public virtual Func<bool> ShouldMakeDirty { get; set; }
    }
}
