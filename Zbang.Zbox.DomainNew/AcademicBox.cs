using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public class AcademicBox : Box
    {
        public AcademicBox(string boxName, User user,
            string courseCode, string professor, Library library, string picture, User creator, string pictureUrl)
            :
            base(boxName, user, BoxPrivacySettings.AnyoneWithUrl)
        {
            if (creator == null) throw new ArgumentNullException("creator");
            InitializeAcademicBox();
            CourseCode = courseCode;
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Library.Add(library);

            Professor = professor;
           // Picture = picture;
            AddPicture(picture, pictureUrl);
            UserTime.CreatedUser = creator.Email;
            var idGenerator = Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();
            Questions.Add(new Comment(creator, Resources.QuestionResource.NewCourse, this, idGenerator.GetId(), null));
            CommentCount = 1;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        protected AcademicBox()
// ReSharper disable once RedundantBaseConstructorCall nhibernate
            : base()
        {
            InitializeAcademicBox();
        }

        private void InitializeAcademicBox()
        {
            Library = new HashSet<Library>();
        }

        public virtual string CourseCode { get; private set; }
        public virtual string Professor { get; private set; }
        public virtual ICollection<Library> Library { get; private set; }

        public void UpdateBoxInfo(string courseCode, string professorName)
        {
            CourseCode = courseCode;
            Professor = professorName;
        }

        public override void GenerateUrl()
        {
            if (Id == 0)
            {
                return;
            }
            Url = UrlConsts.BuildBoxUrl(Id, Name, Owner.GetUniversityName());
        }
    }
}
