using System;

namespace Cloudents.Command.Command
{
    public class AddBuyerTokenCommand : ICommand
    {
        public AddBuyerTokenCommand(long userId, string token, DateTime expiration, string buyerCardMask)
        {
            UserId = userId;
            Token = token;
            Expiration = expiration;
            BuyerCardMask = buyerCardMask;
        }

        public long UserId { get; }
        public string Token { get; }

        public DateTime Expiration { get; }
        public string BuyerCardMask { get; }
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

    public class ConfirmPaymentCommand : ICommand
    {
        public ConfirmPaymentCommand(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; }
    }
}