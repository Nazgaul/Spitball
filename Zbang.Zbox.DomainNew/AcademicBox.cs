using System;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class AcademicBox : Box
    {
        public AcademicBox(string boxName, Library department,
            string courseCode, string professor, User creator, Guid newCommentId)
            :
            base(boxName, creator, BoxPrivacySetting.AnyoneWithUrl)
        {
            University = department.University;
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            CourseCode = courseCode;
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Professor = professor;
            Comments.Add(new Comment(creator, null,
                this, newCommentId, null, FeedType.CreatedCourse, false));
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

        public virtual Library Department { get; set; }

        public virtual University University { get; protected set; }

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
            Url = UrlConst.BuildBoxUrl(Id, Name, University.UniversityName);
        }

        public override string GetUniversityName()
        {
            return University.UniversityName;
        }

        public override void DeleteAssociation()
        {
            Department = null;
            base.DeleteAssociation();
        }
    }
}
