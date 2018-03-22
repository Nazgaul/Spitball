using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Enum
{
    [SuppressMessage("ReSharper", "EnumUnderlyingTypeIsInt", Justification = "Need it for serialization")]
    public enum Vertical : int
    {
        None,
        Note,
        Flashcard,
        Ask,
        Job,
        Tutor,
        Food,
        Book
    }
}