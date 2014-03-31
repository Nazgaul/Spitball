using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public class AcademicBox : Box
    {
        public AcademicBox(string boxName, User user,
            string courseCode, string professor, Library library, string picture, User creator)
            :
            base(boxName, user, BoxPrivacySettings.AnyoneWithUrl)
        {
            InitializeAcademicBox();
            CourseCode = courseCode;
            Library.Add(library);
            Professor = professor;
            Picture = picture;

            var idGenerator = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();
            Questions.Add(new Question(creator, "Created this course", this, idGenerator.GetId(), null));
            CommentCount = 1;
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

        public void UpdateBoxInfo(string courseCode, string professorName)
        {
            CourseCode = courseCode;
            Professor = professorName;
        }
    }
}
