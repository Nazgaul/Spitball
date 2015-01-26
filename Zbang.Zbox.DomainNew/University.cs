
using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Domain.Resources;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public class University : IDirty
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
            AdminNoOfPeople = 10;
            AdminScore = 0;
            UniversityData = this;
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
        public virtual string Country { get; set; }

        public virtual UserTimeDetails UserTime { get; set; }

        public virtual int NoOfBoxes { get; private set; }
        public virtual int NoOfItems { get; set; }
        public virtual int NoOfQuizzes { get; set; }
        public virtual int NoOfUsers { get; set; }

        public virtual ICollection<Library> Libraries { get; protected set; }

        public virtual int AdminScore { get; set; }
        public virtual int AdminNoOfPeople { get; set; }

        public virtual University UniversityData { get; set; }

        public virtual long FacebookUniId { get; set; }




        public virtual void UpdateNumberOfBoxes(int boxesCount)
        {
            if (boxesCount < 0)
            {
                boxesCount = 0;
            }
            NoOfBoxes = boxesCount;
        }

        public virtual Library CreateNewLibraryRoot(Guid id, string nodeName)
        {
            if (UniversityData.Libraries.Any(f => f.Name == nodeName))
            {
                throw new ArgumentException(DomainResources.DeptNameExists);
            }
            var library = new Library(id, nodeName, null, UniversityData);
            Libraries.Add(library);
            return library;
        }

        public bool IsDirty { get; set; }

        public bool IsDeleted { get; set; }

        public string Extra { get; set; }

        public void DeleteAssociation()
        {
        }


        public Func<bool> ShouldMakeDirty
        {
            get
            {
                return () => true;
            }
        }
    }
}
