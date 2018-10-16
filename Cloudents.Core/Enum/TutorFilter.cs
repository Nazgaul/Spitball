using System;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    [Flags]
    public enum TutorFilter
    {
        None = 0,
        [ResourceDescription(typeof(EnumResources), "TutorFilterOnline")]

        Online = 1,
        [ResourceDescription(typeof(EnumResources), "TutorFilterInPerson")]

        InPerson = 2,
    }
}