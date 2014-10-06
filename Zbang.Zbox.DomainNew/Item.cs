using System;
using System.IO;
using Zbang.Zbox.Infrastructure.Consts;
namespace Zbang.Zbox.Domain
{
    public abstract class Item
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

        public virtual Comment Question { get; set; }
        public virtual CommentReplies Answer { get; set; }
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
            //Rate += (rate - Rate) / ++count;
        }

        //public void RevertRate(int previousRate, int count)
        //{
        //    if (count == 1)
        //    {
        //        Rate = 0;
        //        return;
        //    }
        //    Rate -= (previousRate - Rate) / --count;
        //}

        public abstract string ChangeName(string newName);
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
            string thumbnailBlobName, Box box, string thumbnalUrl)
            : base(itemName, uploaderUser, sized, box, blobName, thumbnailBlobName, thumbnalUrl)
        { }

        public void IncreaseNumberOfDownloads()
        {
            NumberOfDownloads++;
        }



        public override string ChangeName(string newName)
        {
            var fileNameExtension = Path.GetExtension(Name);

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newName).RemoveEndOfString(NameLength);
            if (fileNameWithoutExtension == Path.GetFileNameWithoutExtension(Name))
            {
                return Name;
            }

            var newUniquefileName = Box.GetUniqueFileName(fileNameWithoutExtension + fileNameExtension);// command.NewFileName);

            Name = newUniquefileName;
            GenerateUrl();
            return Name;
        }
    }
}
