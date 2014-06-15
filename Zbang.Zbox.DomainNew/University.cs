using System;
using System.Linq;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class University : User
    {
        //public const string UniversityEmailAddress = "support@cloudents.com";
        protected University()
// ReSharper disable once RedundantBaseConstructorCall
            : base()
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Libraries = new HashedSet<Library>();
            NeedCode = false;
        }
        public University(string email, string universityName, string image, string largeImage, string creatingUserName)
            : base(email, universityName, image, largeImage)
        {
            Libraries = new HashedSet<Library>();
            UserTime.CreatedUser = creatingUserName;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            NeedCode = false;
        }

        public University(string universityName, string image, string largeImage, string creatingUserName)
            : this(
            string.Format("{0}@cloudents.com", Guid.NewGuid()),
            universityName, image, largeImage, creatingUserName)
        {
        }

        public virtual ICollection<Library> Libraries { get; set; }
        public virtual bool NeedCode { get; private set; }
// ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual University DataUnversity { get; private set; }


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

        //    if (Name != university.Name) return false;
        //    return true;
        //}
        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        int result;
        //        result = 11 * Name.GetHashCode();

        //        return result;
        //    }
        //}
        #endregion
    }
}
