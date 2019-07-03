namespace Cloudents.Core.Enum
{
    public enum ChatRoomStatus
    {
        All,
        Unassigned,
        Active,
        Scheduled,
        AutoMatchWrong,
        FollowUp,
        NoMatch,
        NeedNewTutor,
        FollowUpFutureS,
        TakenOffline,
        NoTimeTutor,
        WrongSubject,
        FoundOtherTutor,
        ExamDone,
        S_NotInterested,
        S_NewTeacher,
        SessionScheduled,
        SessionDone

    }

    public enum ChatRoomAssign
    {
        All,
        Unassigned,
        Eidan,
        Jaron,
        Yaniv,
        Almog,
        Ron,
        Shira
        
    }

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