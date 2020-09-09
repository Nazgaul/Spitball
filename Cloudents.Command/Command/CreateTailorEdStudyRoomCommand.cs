using System;
using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class CreateTailorEdStudyRoomCommand : ICommand
    {

        public CreateTailorEdStudyRoomCommand(int amountOfUsers)
        {
            //Name = name;
            AmountOfUsers = amountOfUsers;
        }

        public int AmountOfUsers { get; }
        //public string Name { get; }
        public Guid StudyRoomId { get; set; }
        public string Code { get; set; }

        public long TutorId { get; set; }
    }
}