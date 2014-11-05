using System;
using Zbang.Zbox.Infrastructure.Commands;
namespace Zbang.Zbox.Domain.Commands
{
    public class SubscribeToSharedBoxCommand : ICommand
    {
        public SubscribeToSharedBoxCommand(long id, long boxId)
        {
            Id = id;
            BoxId = boxId;
        }

        public long Id { get; private set; }

        public long BoxId { get; private set; }

    }
}