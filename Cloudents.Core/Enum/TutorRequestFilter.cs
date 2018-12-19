using Cloudents.Application.Enum.Resources;
using Cloudents.Common.Attributes;

namespace Cloudents.Application.Enum
{
    public enum TutorRequestFilter
    {
        [ResourceDescription(typeof(EnumResourcesOld), "TutorFilterOnline")]

        Online,
        [ResourceDescription(typeof(EnumResourcesOld), "TutorFilterInPerson")]

        InPerson
    }
}