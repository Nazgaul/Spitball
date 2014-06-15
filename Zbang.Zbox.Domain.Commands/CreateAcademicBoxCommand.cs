using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateAcademicBoxCommand : CreateBoxCommand
    {
        public CreateAcademicBoxCommand(long userId, string boxName, 
            string courseCode, string professor, Guid nodeId)
            :base(userId,boxName,Infrastructure.Enums.BoxPrivacySettings.MembersOnly)
        {
            CourseCode = courseCode;
            Professor = professor;
            NodeId = nodeId;
          //  Department = department;
        }
        public string CourseCode { get; private set; }
        public string Professor { get; private set; }
        public Guid NodeId { get; private set; }
        //public string Department { get; private set; }

    }
}
