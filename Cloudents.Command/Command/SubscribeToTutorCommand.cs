namespace Cloudents.Command.Command
{
    public class SubscribeToTutorCommand : ICommand
    {
        public long UserId { get;  }
        public long TutorId { get;  }

        public SubscribeToTutorCommand(long userId, long tutorId)
        {
            UserId = userId;
            TutorId = tutorId;
        }
    }
}