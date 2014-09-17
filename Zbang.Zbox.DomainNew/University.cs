using System;
using System.Linq;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    //public class University2 : User
    //{
    //    //public const string UniversityEmailAddress = "support@cloudents.com";
    //    protected University2()
    //        // ReSharper disable once RedundantBaseConstructorCall
    //        : base()
    //    {
    //        // ReSharper disable DoNotCallOverridableMethodsInConstructor
    //        NeedCode = false;
    //    }
    //    public University2(string email, string universityName, string image, string largeImage, string creatingUserName)
    //        : base(email, universityName, image, largeImage)
    //    {
    //        UserTime.CreatedUser = creatingUserName;
    //        // ReSharper restore DoNotCallOverridableMethodsInConstructor

    //        NeedCode = false;
    //    }

    //    public University2(string universityName, string image, string largeImage, string creatingUserName)
    //        : this(
    //        string.Format("{0}@cloudents.com", Guid.NewGuid()),
    //        universityName, image, largeImage, creatingUserName)
    //    {
    //    }

    //    public virtual bool NeedCode { get; private set; }
    //    // ReSharper disable UnusedAutoPropertyAccessor.Local
    //    public virtual University2 DataUnversity { get; private set; }


    //    public string WebSiteUrl { get; private set; }
    //    public string MailAddress { get; private set; }
    //    public string FacebookUrl { get; private set; }
    //    public string TwitterUrl { get; private set; }
    //    public long? TwitterWidgetId { get; private set; }
    //    public string YouTubeUrl { get; private set; }
    //    public string LetterUrl { get; set; }
    //    public string AdvertismentUrl { get; private set; }

    //    public virtual string Country { get; set; }

    //    public virtual string UniversityName { get; private set; }
    //    // ReSharper restore UnusedAutoPropertyAccessor.Local

    //    public override string GetUniversityName()
    //    {
    //        return UniversityName;
    //    }
      
    //}


    public class University
    {
        protected University()
        {

        }
        public University(long id, string name, string country, string image, string largeImage, string userEmail)
        {
            Id = id;
            UniversityName = name;
            Country = country;
            Image = image;
            LargeImage = largeImage;
            UserTime = new UserTimeDetails(userEmail);
            NoOfBoxes = 0;
        }

        public virtual long Id { get; set; }
        public string OrgName { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string LargeImage { get; set; }
        public virtual string UniversityName { get; set; }
        public bool NeedCode { get; set; }
        public string WebSiteUrl { get; set; }
        public string MailAddress { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public long? TwitterWidgetId { get; set; }
        public string YouTubeUrl { get; set; }
        public string LetterUrl { get; set; }
        public string AdvertisementUrl { get; set; }
        public string Country { get; set; }

        public virtual UserTimeDetails UserTime { get; set; }

        public virtual int NoOfBoxes { get; private set; }


        public virtual void UpdateNumberOfBoxes(int boxesCount)
        {
            if (boxesCount < 0)
            {
                boxesCount = 0;
            }
            NoOfBoxes = boxesCount;
        }
    }
}
