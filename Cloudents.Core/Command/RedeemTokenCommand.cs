﻿using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command
{
    public class RedeemTokenCommand : ICommand
    {
        public RedeemTokenCommand(long userId, decimal amount)
        {
            UserId = userId;
            Amount = amount;
        }
        

        public long UserId { get; set; }
        public decimal Amount { get; set; }
    }
}