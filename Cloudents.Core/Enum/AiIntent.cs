namespace Cloudents.Core.Enum
{
    public enum AiIntent
    {
        None,
        Qna,
        Tutor,
        [Parse("books")]
        Book, //books
        [Parse("jobs")]
        Job, //jobs
        [Parse("askQuestion")]
        Question, //askQuestion
        Search,
        Purchase
    }
}