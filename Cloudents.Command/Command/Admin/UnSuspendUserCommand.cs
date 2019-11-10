namespace Cloudents.Command.Command.Admin
{
    public class UnSuspendUserCommand : ICommand
    {
        public UnSuspendUserCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }

    }
}
