namespace Cloudents.Command.Courses
{
    public class CreateCourseCommand : ICommand
    {
        public CreateCourseCommand(long userId, string name)
        {
            UserId = userId;
            Name = name;
        }

        public long UserId { get; }
        public string Name { get; }
    }
}