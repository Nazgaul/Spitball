
namespace Cloudents.Command.Command
{
    public class AddPayPalOrderCommand : ICommand
    {
        public AddPayPalOrderCommand(string token)
        {
            Token = token;
        }
        public string Token { get; }
    }
}
