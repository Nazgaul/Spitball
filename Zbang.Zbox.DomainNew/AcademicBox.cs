using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class AcademicBox : Box
    {
        public AcademicBox(string boxName, User user,
            string courseCode, string professor, Library library, string picture)
            :
            base(boxName, user, BoxPrivacySettings.AnyoneWithUrl)
        {
            InitializeAcademicBox();
            CourseCode = courseCode;
            Library.Add(library);
            Professor = professor;
            Picture = picture;
            // Department = department;
        }
        protected AcademicBox()
            : base()
        {
            InitializeAcademicBox();
        }

        private void InitializeAcademicBox()
        {
            Library = new Iesi.Collections.Generic.HashedSet<Library>();
        }

        public virtual string CourseCode { get; private set; }
        public virtual string Professor { get; private set; }
        public virtual ICollection<Library> Library { get; set; }
        //public virtual Department Department { get; private set; }

        public void UpdateBoxInfo(string courseCode, string professorName)
        {
            CourseCode = courseCode;
            Professor = professorName;
        }
    }
}
