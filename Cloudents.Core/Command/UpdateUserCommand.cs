using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class UpdateUserCommand : ICommand
    {
        public UpdateUserCommand(User user)
        {
            User = user;
        }

        public User User { get; private set; }
    }

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



        public string ProviderKey { get; private set; }


        public string ProviderDisplayName { get; private set; }
    }

}