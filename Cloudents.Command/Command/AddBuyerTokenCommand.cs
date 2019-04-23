namespace Cloudents.Command.Command
{
    public class AddBuyerTokenCommand : ICommand
    {
        public AddBuyerTokenCommand(long userId, string token)
        {
            UserId = userId;
            Token = token;
        }

        public long UserId { get;  }
        public string Token { get;  }
    }
}