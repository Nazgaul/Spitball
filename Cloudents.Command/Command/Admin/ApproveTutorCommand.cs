namespace Cloudents.Command.Command.Admin
{
    public class ApproveTutorCommand : ICommand
    {
        public ApproveTutorCommand(long id)
        {
            Id = id;
        }
        public long Id { get; }
    }

    public class BecomeTutorCommand : ICommand
    {
        public BecomeTutorCommand(long id)
        {
            Id = id;
        }
        public long Id { get; }
    }
}
