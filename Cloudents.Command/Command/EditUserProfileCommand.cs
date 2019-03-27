using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command
{
    public class EditUserProfileCommand: ICommand
    {
        public EditUserProfileCommand(long userId, string name, string description)
        {
            UserId = userId;
            Name = name;
            Description = description;
        }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
