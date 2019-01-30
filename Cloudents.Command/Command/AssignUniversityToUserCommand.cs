namespace Cloudents.Command.Command
{
    public class AssignUniversityToUserCommand : ICommand
    {
        public AssignUniversityToUserCommand(long userId, string universityName)
        {
            UserId = userId;
            UniversityName = universityName;
        }

        public long UserId { get; }
        public string UniversityName { get; }

    }
}