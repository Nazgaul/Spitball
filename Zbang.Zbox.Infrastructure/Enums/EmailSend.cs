using System;

namespace Zbang.Zbox.Infrastructure.Enums
{
    [Flags]
    public enum EmailSend
    {
        CanSend = 0,
        Bounce = 1,
        Unsubscribe = 2,
        NoSend = 4,
        Invalid = 8
    }
}