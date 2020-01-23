
namespace Cloudents.Command.Command
{
    public class SetUserGradeCommand : ICommand
    {
        public SetUserGradeCommand(long userId, short grade)
        {
            UserId = userId;
            Grade = grade;
        }
        public long UserId { get; set; }
        public short Grade { get; set; }
    }
}
