namespace Cloudents.Core.Enum
{
    public enum ChatRoomStatus
    {
        Default,
        Active,
        Scheduled,
        NoMatch,
        Archive,
        Unassigned
    }

    public enum ChatRoomAssign
    {
        None,
        Eidan,
        Jaron,
        Yaniv,
        Almog,
        Ron,
        Unassigned
    }

    public enum WaitingFor
    {
        Default = 0,
        Tutor = 1,
        Student = 2,
        Conv = 3
    }
}