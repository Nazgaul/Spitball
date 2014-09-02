
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateDepartmentCommand : ICommand
    {
        public CreateDepartmentCommand(string name, long universityId, long userId)
        {
            UserId = userId;
            Name = name;
            UniversityId = universityId;
        }

        public string Name { get; private set; }
        public long UniversityId { get; private set; }

        public long UserId { get; private set; }


        public long Id { get; set; }
    }

}
