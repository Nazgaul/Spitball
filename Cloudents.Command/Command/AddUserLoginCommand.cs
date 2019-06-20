using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class AddUserLoginCommand : ICommand
    {
        public AddUserLoginCommand(User user, string loginProvider, string providerKey, string providerDisplayName)
        {
            User = user;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
            ProviderDisplayName = providerDisplayName;
        }

        public User User { get; }

        public string LoginProvider { get; }

        public string ProviderKey { get; }


        public string ProviderDisplayName { get; }
    }
}