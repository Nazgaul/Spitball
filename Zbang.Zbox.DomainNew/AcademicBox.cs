using System;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public class AcademicBox : Box
    {
        public AcademicBox(string boxName, Library department,
            string courseCode, string professor, string picture, User creator, string pictureUrl)
            :
            base(boxName, creator, BoxPrivacySettings.AnyoneWithUrl)
        {
            University = department.University;
            if (creator == null) throw new ArgumentNullException("creator");
            CourseCode = courseCode;
            // ReSharper disable DoNotCallOverridableMethodsInConstructor

            Professor = professor;
            // Picture = picture;
            AddPicture(picture, pictureUrl);
            UserTime.CreatedUser = creator.Email;
            var idGenerator = Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();

            Resources.QuestionResource.Culture = Languages.GetCultureBaseOnCountry(University.Country);

            Questions.Add(new Comment(creator, Resources.QuestionResource.NewCourse, this, idGenerator.GetId(), null));
            CommentCount = 1;

            Department = department;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        protected AcademicBox()
            // ReSharper disable once RedundantBaseConstructorCall nhibernate
            : base()
        {

        }


        public virtual string CourseCode { get; private set; }
        public virtual string Professor { get; private set; }

        public virtual Library Department { get; private set; }

        public virtual University University { get; private set; }

        public virtual void UpdateDepartment(Library dep, University university)
        {
            Department = dep;
            University = university;
        }

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
            Url = UrlConsts.BuildBoxUrl(Id, Name, University.UniversityName);
        }
    }
}
