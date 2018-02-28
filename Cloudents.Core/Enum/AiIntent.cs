using Cloudents.Core.Attributes;

namespace Cloudents.Core.Enum
{
    public enum AiIntent
    {
        None,
        Tutor,
        [Parse("books")]
        Book,
        [Parse("jobs")]
        Job,
        [Parse("askQuestion")]
        Question,
        Search,
        Purchase
    }
}