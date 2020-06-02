namespace Cloudents.Command.Command
{
    public class AddSellerTokenCommand : ICommand
    {
        public AddSellerTokenCommand(string userEmail, string token)
        {
            UserEmail = userEmail;
            Token = token;
        }

        public string UserEmail { get; }
        public string Token { get; }

    }
}