using Zbang.Zbox.Infrastructure.Commands;
namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserUniversityCommand : ICommand
    {
        public UpdateUserUniversityCommand(long universityId, 
            long userId, 
            string studentId)
        {
            UniversityId = universityId;
            UserId = userId;
         
            StudentId = studentId;
        }

        

        public long UserId { get; private set; }

        public string StudentId { get; private set; }

        public long? UniversityDataId { get; set; }
        public long UniversityId { get; set; }
    }
}
