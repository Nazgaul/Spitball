namespace Cloudents.Command.Command
{
    public class AddStripeCustomerCommand : ICommand
    {
        public AddStripeCustomerCommand(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; }


        public string ClientSecretId { get; set; }
    }

    
}