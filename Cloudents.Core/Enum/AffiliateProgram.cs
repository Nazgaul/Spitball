namespace Cloudents.Core.Enum
{
    public enum AffiliateProgram
    {
        None,
        //CareerBuilder,
        WayUp,
        Wyzant
    }

    public enum TransactionType
    {
        //None,
        Awarded,
        Earned,
        Pending,
        Stake,
        Spent
    }

    public enum ActionType
    {
        None,
        SignUp,
        Question,
        DeleteQuestion,
        Answer,
        QuestionCorrect,
        CashOut
    }
}