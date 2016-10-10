using System;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateNodeSettingsCommand : ICommand
    {
        public UpdateNodeSettingsCommand(string newName, Guid nodeId, LibraryNodeSetting? settings, long userId)
        {
            UserId = userId;
            Settings = settings;
            NodeId = nodeId;
            NewName = newName;
        }

        public string NewName { get; private set; }
        public Guid NodeId { get; private set; }
        public long UserId { get; private set; }
        public LibraryNodeSetting? Settings { get; private set; }
    }
}
