using System;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
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