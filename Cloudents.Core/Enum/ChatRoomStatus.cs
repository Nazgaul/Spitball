using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.Enum
{

    public enum PaymentStatus :int
    {
        None,
        Done,
        //Later

    }
    //public enum ChatRoomStatus
    //{
    //    New,
    //    NeedDocs
    //    All,
    //    Unassigned,
    //    Active,
    //    Scheduled,
    //    AutoMatchWrong,
    //    FollowUp,
    //    NoMatch,
    //    NeedNewTutor,
    //    FollowUpFutureS,
    //    TakenOffline,
    //    NoTimeTutor,
    //    WrongSubject,
    //    FoundOtherTutor,
    //    ExamDone,
    //    S_NotInterested,
    //    S_NewTeacher,
    //    SessionScheduled,
    //    SessionDone

    //}

    public class ChatRoomStatus : Enumeration
    {
        public string Group { get; }
        private const string CNew = "New";
        private const string CActive = "Active";
        private const string CDelete = "Delete";

        //public static ChatRoomStatus All = new ChatRoomStatus("All","All");
        public static readonly ChatRoomStatus New = new ChatRoomStatus(1, "New", CNew);

        public static readonly ChatRoomStatus NeedDocs = new ChatRoomStatus(2, "Need docs", CActive);
        public static readonly ChatRoomStatus ReceivedDocs = new ChatRoomStatus(3, "Received docs", CActive);
        public static readonly ChatRoomStatus TakenOffline = new ChatRoomStatus(4, "Taken offline", CActive);
        public static readonly ChatRoomStatus Scheduling = new ChatRoomStatus(5, "Scheduling", CActive);
        public static readonly ChatRoomStatus ReminderToTutor = new ChatRoomStatus(6, "Reminder to Tutor ", CActive);
        public static readonly ChatRoomStatus ReminderToStudent = new ChatRoomStatus(7, "Reminder to Student", CActive);

        public static readonly ChatRoomStatus SessionScheduled = new ChatRoomStatus(8, "Session Scheduled", CDelete);
        public static readonly ChatRoomStatus TutorNoTime = new ChatRoomStatus(9, "Tutor has no time", CDelete);
        public static readonly ChatRoomStatus WrongAutoMatch = new ChatRoomStatus(10, "Wrong Auto match", CDelete);
        public static readonly ChatRoomStatus ActiveOtherTutor = new ChatRoomStatus(11, "Active w/ other tutor", CDelete);
        public static readonly ChatRoomStatus StudentNotRelevant = new ChatRoomStatus(12, "Stud. Not relevant", CDelete);
        public static readonly ChatRoomStatus TutorNotResponding = new ChatRoomStatus(13, "Tutor not responding", CDelete);
        public static readonly ChatRoomStatus StudentNotResponding = new ChatRoomStatus(14, "Stud. not responding", CDelete);
        public static readonly ChatRoomStatus DoNotKnowMaterial = new ChatRoomStatus(15, "Tutor doesn't know material ", CDelete);
        public static readonly ChatRoomStatus Spam = new ChatRoomStatus(16, "Spam", CDelete);
        public static readonly ChatRoomStatus Na = new ChatRoomStatus(17, "N/A", CDelete);

        private ChatRoomStatus(int id, string name, string group) : base(id, name)
        {
            Group = group;
        }

        public static IEnumerable<ChatRoomStatus> GetActiveStatus()
        {
            return GetAll<ChatRoomStatus>().Where(w => w.Group == CActive || w.Group == CNew);
        }


    }



    //public enum ChatRoomAssign
    //{
    //    All,
    //    Unassigned,
    //    Eidan,
    //    Jaron,
    //    Yaniv,
    //    Almog,
    //    Ron,
    //    Shira
    //    Gilad

    //}

    public enum WaitingFor
    {
        All = 0,
        Tutor = 1,
        Student = 2,
        Conv = 3
    }

    public enum StudyRoomType
    {
        SmallGroup,
        PeerToPeer
    }
}