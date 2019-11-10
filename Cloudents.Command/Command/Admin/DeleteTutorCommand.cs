
namespace Cloudents.Command.Command.Admin
{
    public class DeleteTutorCommand : ICommand
    {
        public DeleteTutorCommand(long id)
        {
            Id = id;
        }
        public long Id { get; }
    }
}
