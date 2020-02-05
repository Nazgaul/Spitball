
namespace Cloudents.Core.Enum
{
    public enum DocumentType
    {
        Document = 0,
        Video = 1
    }

    [System.Flags]
    public enum FeedType
    {
        Document = 0,
        Video = 1,
        Question = 2,
        Tutor = 4,

        All = Document | Video | Question | Tutor
    }
}
