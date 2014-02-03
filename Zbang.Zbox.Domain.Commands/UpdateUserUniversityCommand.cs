using Zbang.Zbox.Infrastructure.Commands;
namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserUniversityCommand : ICommand
    {
        public UpdateUserUniversityCommand(long universityId, long userId, string code = null)
        {
            UniversityId = universityId;
            UserId = userId;
            Code = code;
        }

        public long UniversityId { get; set; }

        public long UserId { get; private set; }
        public string Code { get; private set; }

        public long? UniversityWrapperId { get; set; }
    }
}
