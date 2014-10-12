using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateAcademicBoxCommand : CreateBoxCommand
    {
        public CreateAcademicBoxCommand(long userId, string boxName,
            string courseCode, string professor, Guid departmentId)
            : base(userId, boxName)
        {
            CourseCode = courseCode;
            Professor = professor;
            DepartmentId = departmentId;
        }
        public string CourseCode { get; private set; }
        public string Professor { get; private set; }
        public Guid DepartmentId { get; private set; }

    }
}
