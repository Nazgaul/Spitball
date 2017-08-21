using System;

namespace Zbang.Zbox.Domain
{
    public class AcademicBoxClosed : AcademicBox
    {
        public AcademicBoxClosed(string boxName, Library department,
            string courseCode, string professor, User creator, Guid newCommentId, Library parentDepartment)
            :
            base(boxName, department, courseCode, professor, creator, newCommentId)
        {
            ParentDepartment = parentDepartment;
        }

        protected AcademicBoxClosed()
            : base()
        {

        }

        public Library ParentDepartment { get; set; }

        public Guid ParentDepartmentId { get; set; }
    }
}
