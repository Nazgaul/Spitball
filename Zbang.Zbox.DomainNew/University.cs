
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class University
    {
        protected University()
        {

        }
        public University(long id, string name, string country, string largeImage, string userEmail)
        {
            Id = id;
            UniversityName = name;
            Country = country;
            LargeImage = largeImage;
            UserTime = new UserTimeDetails(userEmail);
            NoOfBoxes = 0;
        }

        public virtual long Id { get; set; }
        public string OrgName { get; set; }
        public string Url { get; set; }
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

        protected virtual ICollection<Library> Libraries { get; set; }


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
