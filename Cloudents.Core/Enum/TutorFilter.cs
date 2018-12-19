using System;

namespace Cloudents.Application.Enum
{
    //TODO: why to we need TutorFilter and TutorRequestFilter
    [Flags]
    public enum TutorFilter
    {
        None = 0,

        Online = 1,

        InPerson = 2,
    }
}