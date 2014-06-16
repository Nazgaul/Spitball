﻿using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteFileFromQnACommand : ICommand
    {
        public DeleteFileFromQnACommand(long itemId, long userId)
        {
            ItemId = itemId;
            UserId = userId;
        }


        public long ItemId { get; set; }

        public long UserId { get; set; }
    }
}
