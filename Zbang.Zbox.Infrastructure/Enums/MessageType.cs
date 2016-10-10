﻿
namespace Zbang.Zbox.Infrastructure.Enums
{
    // ReSharper disable once EnumUnderlyingTypeIsInt - need for dapper
    public enum MessageType : int
    {
        None = 0,
        Invite = 2,
        InviteToSystem = 3
    }
}
