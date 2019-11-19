namespace Cloudents.Command.Command.Admin
{
    public class DistributeTokensCommand : ICommand
    {
        public DistributeTokensCommand(long userId, decimal price)
        {
            UserId = userId;
            Price = price;
        }

        public long UserId { get; }
        public decimal Price { get; }


    }
}
