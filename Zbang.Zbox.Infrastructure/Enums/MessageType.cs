
namespace Zbang.Zbox.Infrastructure.Enums
{
    // ReSharper disable once EnumUnderlyingTypeIsInt - need for dapper
    //We need this because of mapping of nhibernate
    public enum MessageType : int
    {
        None = 0,
        Invite = 2,
        InviteToSystem = 3
    }
}
