using System;
using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class CreateTailorEdStudyRoomCommand : ICommand
    {

        public CreateTailorEdStudyRoomCommand(string name, int amountOfUsers)
        {
            Name = name;
            AmountOfUsers = amountOfUsers;
        }

        public int AmountOfUsers { get; }
        public string Name { get; }
        public Guid StudyRoomId { get; set; }
        public IEnumerable<string> Codes { get; set; } 
    }
}