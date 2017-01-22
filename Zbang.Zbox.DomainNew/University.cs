
using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Enums;
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
            CreateUtcBaseOnCountry();
            UserTime = new UserTimeDetails(userId);
            NoOfBoxes = 0;
            AdminNoOfPeople = 10;
            AdminScore = 0;
            UniversityData = this;
        }

        private void CreateUtcBaseOnCountry()
        {
            if (Country == "IL")
            {
                UtcOffset = 3;
            }
            if (Country == "US")
            {
                UtcOffset = -5;
            }
        }

        public virtual long Id { get; set; }
        public string OrgName { get; set; }
        public string Url { get; set; }
        public string LargeImage { get; set; }
        public virtual string UniversityName { get; set; }
        
        public virtual string Country { get; set; }

        public virtual UserTimeDetails UserTime { get; set; }

        public virtual int NoOfBoxes { get; private set; }
        public virtual int NoOfItems { get; set; }
        public virtual int NoOfQuizzes { get; set; }
        public virtual int NoOfUsers { get; set; }
        public virtual int NoOfFlashcards { get; set; }

        public virtual ICollection<Library> Libraries { get; protected set; }

        public virtual int AdminScore { get; set; }
        public virtual int AdminNoOfPeople { get; set; }

        public virtual University UniversityData { get; set; }

        public virtual long FacebookUniId { get; set; }

        public string TextPopupUpper { get; set; }
        public string TextPopupLower { get; set; }


        public string HeaderBackgroundColor { get; set; }

        public string BackgroundImage { get; set; }


        public string VideoBackgroundColor { get; set; }

        public string VideoFontColor { get; set; }

        public string SignupColor { get; set; }

        public string MainSignupColor { get; set; }

        public int? UtcOffset { get; set; }

        public float? Latitude { get; set; }
        public float? Longitude { get; set; }

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
