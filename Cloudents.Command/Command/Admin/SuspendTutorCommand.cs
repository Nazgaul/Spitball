
namespace Cloudents.Command.Command.Admin
{
    public class SuspendTutorCommand : ICommand
    {
        public long TutorId { get; set; }
        public SuspendTutorCommand(long tutorId)
        {
            TutorId = tutorId;
        }
    }
}
