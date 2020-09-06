using System;

namespace Cloudents.Core.Interfaces
{
    [Flags]
    public enum JsonConverterTypes
    {
        None = 0x0,
        TimeSpan = 0x1
    }
}