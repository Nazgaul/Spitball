using Cloudents.Common.Attributes;
using Cloudents.Common.Resources;

namespace Cloudents.Application.Enum
{
    public enum TutorRequestFilter
    {
        [ResourceDescription(typeof(EnumResources), "TutorFilterOnline")]

        Online,
        [ResourceDescription(typeof(EnumResources), "TutorFilterInPerson")]

        InPerson
    }
}