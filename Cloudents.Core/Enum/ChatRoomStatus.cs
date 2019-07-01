namespace Cloudents.Core.Enum
{
    public enum ChatRoomStatus
    {
        Unassigned,
        Active,
        Scheduled,
        AutoMatchWrong,
        FollowUp,
        NoMatch,
        NeedNewTutor,
        All

}

    public enum ChatRoomAssign
    {
        Unassigned,
        Eidan,
        Jaron,
        Yaniv,
        Almog,
        Ron,
        Shira,
        All
    }

    public enum WaitingFor
    {
        All = 0,
        Tutor = 1,
        Student = 2,
        Conv = 3
    }
}