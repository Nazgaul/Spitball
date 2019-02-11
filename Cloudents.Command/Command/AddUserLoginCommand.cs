using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class AddUserLoginCommand : ICommand
    {
        public AddUserLoginCommand(RegularUser user, string loginProvider, string providerKey, string providerDisplayName)
        {
            User = user;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
            ProviderDisplayName = providerDisplayName;
        }

        public RegularUser User { get; }

        public string LoginProvider { get; }

        public string ProviderKey { get; }


        public string ProviderDisplayName { get; }
    }
}