﻿using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Command
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

        public string ProviderKey { get; private set; }


        public string ProviderDisplayName { get; private set; }
    }
}