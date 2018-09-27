using Cloudents.Core.Attributes;

namespace Cloudents.Core.Enum
{
    public enum QuestionColor
    {
        Default,
        Green,
        Purple,
        Red,
        Yellow,
        Blue,
        Turquoise,
        LightBlue,
        Grey,
        Olive,
        Pink
    }

    public enum QuestionFilter
    {
        All,
        [PublicValue]
        Unanswered,
        [PublicValue]
        Answered,
        [PublicValue]
        Sold
    }
}