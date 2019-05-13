using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class PaymentCommand: ICommand
    {
        public PaymentCommand(string userKey, string tutorKey, decimal anount, Guid studyRoomSessionId)
        {
            UserKey = userKey;
            TutorKey = tutorKey;
            Anount = anount;
            StudyRoomSessionId = studyRoomSessionId;
        }
        public string UserKey { get; set; }
        public string TutorKey { get; set; }
        public decimal Anount { get; set; }
        public Guid StudyRoomSessionId { get; set; }
    }
}
