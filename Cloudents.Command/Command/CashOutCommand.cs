namespace Cloudents.Command.Command
{
    public class CashOutCommand : ICommand
    {
        public CashOutCommand(long userId/*, decimal amount*/)
        {
            UserId = userId;
            // Amount = amount;
        }


        public long UserId { get; set; }
        //public decimal Amount { get; set; }
    }
}