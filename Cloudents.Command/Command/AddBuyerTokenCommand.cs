﻿using System;

namespace Cloudents.Command.Command
{
    public class AddBuyerTokenCommand : ICommand
    {
        public AddBuyerTokenCommand(long userId, string token, DateTime expiration)
        {
            UserId = userId;
            Token = token;
            Expiration = expiration;
        }

        public long UserId { get;  }
        public string Token { get;  }

        public DateTime Expiration { get;  }
    }

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