using Zbang.Zbox.Infrastructure.Commands;
namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserUniversityCommand : ICommand
    {
        public UpdateUserUniversityCommand(long universityId, 
            long userId, long? departmentId,
            string code = null,
            string groupNumber = null,
            string registerNumber = null)
        {
            UniversityId = universityId;
            UserId = userId;
            Code = code;
            DepartmentId = departmentId.GetValueOrDefault();
            GroupNumber = groupNumber;
            RegisterNumber = registerNumber;
        }

        public long UniversityId { get; set; }

        public long UserId { get; private set; }
        public string Code { get; private set; }

        public long? UniversityWrapperId { get; set; }

        public long DepartmentId { get; set; }

        public string GroupNumber { get; private set; }
        public string RegisterNumber { get; private set; }
    }
}
