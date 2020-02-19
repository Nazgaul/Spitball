namespace Cloudents.Command.Command.Admin
{
    public class ChangeTutorPriceCommand : ICommand
    {
        public ChangeTutorPriceCommand(long tutorId, decimal price)
        {
            TutorId = tutorId;
            Price = price;
        }

        public long TutorId { get;  }
        public decimal Price { get;  }
    }
}