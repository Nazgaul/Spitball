namespace Cloudents.Core.Enum
{
    public enum SeoType
    {
        Static,
        [Parse(SeoTypeString.Item)]
        Item,
        Quiz,
        Flashcard
    }

    public static class SeoTypeString
    {
        public const string Item = "Item";
    }
}
