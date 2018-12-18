using Cloudents.Common.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    public enum TutorRequestFilter
    {
        [ResourceDescription(typeof(EnumResourcesOld), "TutorFilterOnline")]

        Online,
        [ResourceDescription(typeof(EnumResourcesOld), "TutorFilterInPerson")]

        InPerson
    }
}