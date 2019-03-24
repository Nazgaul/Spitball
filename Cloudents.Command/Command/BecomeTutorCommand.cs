namespace Cloudents.Command.Command
{
    public class BecomeTutorCommand : ICommand
    {
        public BecomeTutorCommand(long userId, decimal price, string bio)
        {
            UserId = userId;
            Price = price;
            Bio = bio;
        }

        public long UserId { get; private set; }

        public decimal Price { get; private set; }

        public string Bio { get; private set; }
    }
}