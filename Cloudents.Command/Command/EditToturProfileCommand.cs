using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command
{
    public class EditToturProfileCommand: ICommand
    {
        public EditToturProfileCommand(long userId, string name, string lastName, string bio, string description)
        {
            UserId = userId;
            Name = name;
            LastName = lastName;
            Bio = bio;
            Description = description;
        }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string Description { get; set; }
    }
}
