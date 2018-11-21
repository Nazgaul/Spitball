using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    public enum DocumentType
    {
        None,
        [PublicValue]
        Lecture,
        [PublicValue]
        Textbook,
        [PublicValue]
        Exam
    }
}