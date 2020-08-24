namespace Cloudents.Command.Command
{
    public class BecomeTutorCommand : ICommand
    {
        public BecomeTutorCommand(long id)
        {
            Id = id;
        }
        public long Id { get; }
    }
}