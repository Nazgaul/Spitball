using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateAcademicBoxCommand : CreateBoxCommand
    {
        public CreateAcademicBoxCommand(long userId, string boxName, 
            string courseCode, string professor, long departmentId)
            :base(userId,boxName,Infrastructure.Enums.BoxPrivacySettings.MembersOnly)
        {
            CourseCode = courseCode;
            Professor = professor;
            DepartmentId = departmentId;
        }
        public string CourseCode { get; private set; }
        public string Professor { get; private set; }
        public long DepartmentId { get; private set; }

    }
}
