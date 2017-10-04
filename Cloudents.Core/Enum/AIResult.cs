namespace Cloudents.Core.Enum
{
    public enum AIResult
    {
        None,
        SearchOrQuestion,
        Search,
        AddSubjectOrCourse,
        AddSubject,
        Tutor,
        Book,
        Job,
        ChatPost,
        Question,
        Qna,
        AddSearchTypeToSubject, //Need to change Dto
        Purchase,
        PurchaseAskBuy,
        PurchaseChangeTerm //Need to change Dto

    }

    public enum AIIntent
    {
        None,
        Qna,
        Tutor,
        Book, //books
        Job, //jobs
        Question, //askQuestion
        Search,
        Purchase
    }
}