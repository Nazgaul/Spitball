
namespace Cloudents.Command.Command
{
    public class AddPayPalOrderCommand : ICommand
    {
        public AddPayPalOrderCommand(long userId, string token)
        {
            Token = token;
            UserId = userId;
        }
        public string Token { get; }
        public long UserId { get; }
    }
}
