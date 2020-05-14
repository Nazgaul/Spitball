namespace Cloudents.Command.Command
{
    public class CreateTutorSubscriptionCommand : ICommand
    {
        public CreateTutorSubscriptionCommand(long tutorId, double price)
        {
            TutorId = tutorId;
            Price = price;
        }

        public long TutorId { get;  }

        public double Price { get;  }
    }
}