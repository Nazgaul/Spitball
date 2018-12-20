using Cloudents.Common.Attributes;
using Cloudents.Core.Enum.Resources;

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