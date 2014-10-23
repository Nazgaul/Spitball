using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateUserCommandResult: ICommandResult
    {
        public CreateUserCommandResult(User user)
        {
            User = user;
        }

        public User User { get; set; }

        public long? UniversityId { get; set; }

        public long? UniversityData { get; set; }
    }
}
