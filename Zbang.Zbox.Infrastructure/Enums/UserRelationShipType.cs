
namespace Zbang.Zbox.Infrastructure.Enums
{
// ReSharper disable once EnumUnderlyingTypeIsInt - need for dapper
    public enum UserRelationshipType : int
    {
        None = 0,
        Invite = 1,
        Subscribe = 2,
        Owner = 3
    }

    public enum UserLibraryRelationType : int
    {
        None = 0,
        Pending = 1,
        Subscribe = 2,
        Owner = 3
    }
}
