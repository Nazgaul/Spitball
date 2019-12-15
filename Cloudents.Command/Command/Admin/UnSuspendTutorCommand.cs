namespace Cloudents.Command.Command.Admin
{
    public class UnSuspendTutorCommand : ICommand
    {
        public long TutorId { get; set; }
        public UnSuspendTutorCommand(long tutorId)
        {
            TutorId = tutorId;
        }
    }
}
