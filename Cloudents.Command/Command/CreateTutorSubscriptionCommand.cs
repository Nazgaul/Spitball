namespace Cloudents.Command.Command
{
    public class CreateTutorSubscriptionCommand : ICommand
    {
        public CreateTutorSubscriptionCommand(long tutorId, decimal price)
        {
            TutorId = tutorId;
            Price = price;
        }

        public long TutorId { get;  }

        public decimal Price { get;  }
    }
}