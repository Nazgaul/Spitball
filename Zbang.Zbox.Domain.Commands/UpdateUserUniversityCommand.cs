using Zbang.Zbox.Infrastructure.Commands;
namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserUniversityCommand : ICommand
    {
        public UpdateUserUniversityCommand(long universityId, 
            long userId, long? departmentId,
            string code ,
            string groupNumber,
            string registerNumber,
            string studentId)
        {
            UniversityId = universityId;
            UserId = userId;
            Code = code;
            DepartmentId = departmentId.GetValueOrDefault();
            GroupNumber = groupNumber;
            RegisterNumber = registerNumber;
            StudentId = studentId;
        }

        

        public long UserId { get; private set; }
        public string Code { get; private set; }


        public long DepartmentId { get; private set; }

        public string GroupNumber { get; private set; }
        public string RegisterNumber { get; private set; }

        public string StudentId { get; private set; }

        public long?UniversityDataId { get; set; }
        public long UniversityId { get; set; }
    }
}
