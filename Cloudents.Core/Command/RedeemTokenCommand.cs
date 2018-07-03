using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class RedeemTokenCommand : ICommand
    {
        public RedeemTokenCommand(long userId, decimal amount)
        {
            UserId = userId;
            Amount = amount;
        }
        public RedeemTokenCommand()
        {
            
        }

        public long UserId { get; set; }
        public decimal Amount { get; set; }
    }
}