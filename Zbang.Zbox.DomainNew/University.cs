﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public class University : IDirty
    {
        protected University()
        {
            ShouldMakeDirty = () => true;
        }
        public University(long id, string name, string country, long userId)
        {
            Id = id;
            UniversityName = name;
            Country = country;
            UserTime = new UserTimeDetails(userId);
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
        //public string WebSiteUrl { get; set; }
        //public string MailAddress { get; set; }
        //public string FacebookUrl { get; set; }
        //public string TwitterUrl { get; set; }
        //public long? TwitterWidgetId { get; set; }
        //public string YouTubeUrl { get; set; }
        //public string LetterUrl { get; set; }
        //public string AdvertisementUrl { get; set; }
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

        public string TextPopupUpper { get; set; }
        public string TextPopupLower { get; set; }




        public virtual void UpdateNumberOfBoxes(int boxesCount)
        {
            if (boxesCount < 0)
            {
                boxesCount = 0;
            }
            NoOfBoxes = boxesCount;
        }

        public virtual Library CreateNewLibraryRoot(Guid id, string nodeName, User user)
        {
            if (UniversityData.Libraries.Any(f => f.Name == nodeName))
            {
                throw new DuplicateDepartmentNameException();
            }
            var library = new Library(id, nodeName, UniversityData, user);
            Libraries.Add(library);
            return library;
        }

        public bool IsDirty { get; set; }

        public bool IsDeleted { get; set; }

        public string Extra { get; set; }

        public void DeleteAssociation()
        {
        }


        public virtual Func<bool> ShouldMakeDirty { get; set; }
    }
}
