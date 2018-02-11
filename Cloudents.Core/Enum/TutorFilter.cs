using System;

namespace Cloudents.Core.Enum
{
    [Flags]
    public enum TutorFilter
    {
        None = 0,
        Online = 1,
        InPerson = 2,
    }
}