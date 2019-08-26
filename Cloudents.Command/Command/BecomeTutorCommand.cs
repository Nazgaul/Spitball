namespace Cloudents.Command.Command
{
    public class BecomeTutorCommand : ICommand
    {
        public BecomeTutorCommand(long userId, string firstName, string lastName,
            string description, string bio, int price)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Description = description;
            Bio = bio;
            Price = price;
        }

        public long UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Description { get; }

        public string Bio { get;  }
        public int Price { get;  }
    }
   
}