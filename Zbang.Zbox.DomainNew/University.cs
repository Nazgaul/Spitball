using System;
using System.Linq;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class University2 : User
    {
        //public const string UniversityEmailAddress = "support@cloudents.com";
        protected University2()
            // ReSharper disable once RedundantBaseConstructorCall
            : base()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Libraries = new HashSet<Library>();
            NeedCode = false;
        }
        public University2(string email, string universityName, string image, string largeImage, string creatingUserName)
            : base(email, universityName, image, largeImage)
        {
            Libraries = new HashSet<Library>();
            UserTime.CreatedUser = creatingUserName;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            NeedCode = false;
        }

        public University2(string universityName, string image, string largeImage, string creatingUserName)
            : this(
            string.Format("{0}@cloudents.com", Guid.NewGuid()),
            universityName, image, largeImage, creatingUserName)
        {
        }

        public virtual ICollection<Library> Libraries { get; set; }
        public virtual bool NeedCode { get; private set; }
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual University2 DataUnversity { get; private set; }


        public string WebSiteUrl { get; private set; }
        public string MailAddress { get; private set; }
        public string FacebookUrl { get; private set; }
        public string TwitterUrl { get; private set; }
        public long? TwitterWidgetId { get; private set; }
        public string YouTubeUrl { get; private set; }
        public string LetterUrl { get; set; }
        public string AdvertismentUrl { get; private set; }

        public virtual string Country { get; set; }

        public virtual string UniversityName { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        public override string GetUniversityName()
        {
            return UniversityName;
        }
        public virtual Library CreateNewLibraryRoot(Guid id, string nodeName)
        {
            if (Libraries.Any(f => f.Name == nodeName))
            {
                throw new ArgumentException("cannot have node with the same name");
            }
            var library = new Library(id, nodeName, null, this);
            Libraries.Add(library);
            return library;
        }


        #region Nhibernate
        //public override bool Equals(object obj)
        //{
        //    if (this == obj) return true;

        //    var university = obj as University;
        //    if (university == null) return false;

        //    if (UniversityName != university.UniversityName) return false;
        //    return true;
        //}
        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        int result;
        //        result = 11 * UniversityName.GetHashCode();

        //        return result;
        //    }
        //}
        #endregion
    }


    public class University
    {
        protected University()
        {

        }
        public University(long id, string name, string country, string image, string largeImage)
        {
            Id = id;
            UniversityName = name;
            Country = country;
            Image = image;
            LargeImage = largeImage;
            UserTime = new UserTimeDetails("sys");
        }

        public long Id { get; set; }
        public string OrgName { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string LargeImage { get; set; }
        public string UniversityName { get; set; }
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


    }
}
