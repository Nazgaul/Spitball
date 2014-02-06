using System;
using System.Collections.Generic;
using System.Linq;

namespace Zbang.Zbox.Domain
{
    public class Item
    {
        public const int NameLength = 120;
        protected Item()
        {
            IsDeleted = false;
        }
        public Item(string itemName, User uploader, long iSized, Box box, string itemContentUrl)
            : this()
        {
            DateTimeUser = new UserTimeDetails(uploader.Email);
            Name = itemName;
            Uploader = uploader;
            Size = iSized;
            Box = box;
            ItemContentUrl = itemContentUrl;

        }
        public virtual long Id { get; protected set; }

        public virtual string Name { get; set; }
        public virtual UserTimeDetails DateTimeUser { get; protected set; }
        public virtual bool IsDeleted { get; set; }

        public virtual User Uploader { get; set; }
        public virtual long Size { get; set; }
        public virtual Box Box { get; set; }
        public virtual int NumberOfViews { get; private set; }

        public virtual Question Question { get; set; }
        public virtual Answer Answer { get; set; }
        public virtual string ItemContentUrl { get; set; }

        public virtual float Rate { get; internal set; }

        public void IncreaseNumberOfViews()
        {
            NumberOfViews++;
        }


        public virtual void CalcalateRate(int rate, int count)
        {
            Rate += (rate - Rate) / ++count;
        }

        public void RevertRate(int prevRate, int count)
        {
            if (count == 1)
            {
                Rate = 0;
                return;
            }
            Rate -= (prevRate - Rate) / --count;
        }

        //public virtual string Uid
        //{
        //    get
        //    {
        //        if (Id == 0)
        //        {
        //            return null;
        //        }
        //        var shortUrlDecoder = Infrastructure.Url.ShortCodesCache.Create();
        //        return shortUrlDecoder.LongToShortCode(Id, Infrastructure.Url.ShortCodesType.Item);
        //    }
        //}
    }

    public class Link : Item
    {
        protected Link()
        {

        }
        public Link(string itemName, User iUploaderUser, long iSized, Box box, string linkTitle)
            : base(linkTitle, iUploaderUser, iSized, box, itemName)
        { }
    }

    public class File : Item
    {

        public virtual string ThumbnailBlobName { get; set; }
        public virtual int NumberOfDownloads { get; private set; }

        public virtual string Content { get; set; }


        protected File()
        {
        }

        public File(string iItemName, User iUploaderUser, long iSized, string iBlobName, string iThumbnailBlobName, Box box)
            : base(iItemName, iUploaderUser, iSized, box, iBlobName)
        {

            ThumbnailBlobName = iThumbnailBlobName;
        }

        public void IncreaseNumberOfDownloads()
        {
            NumberOfDownloads++;
        }


    }
}
