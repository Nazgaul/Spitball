
using System;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum UserType
    {
        Regular = 0,
        TooHighScore = 1
    }

    public enum Platform
    {
        Default,
        Desktop,
        MobileWeb,
        MobileApp
    }

    public enum MobileOperatingSystem
    {
        None = 0,
        Android = 1,
// ReSharper disable once InconsistentNaming
        iOS = 2
    }

    [Flags]
    public enum PushNotificationSettings
    {
        None = 0,
        On = 1
    }

    [Flags]
    public enum EmailSend
    {
        CanSend = 0,
        Bounce = 1,
        Unsubscribe = 2,
        NoSend = 4
    }
}
