using System;

namespace Cloudents.Core.Request
{

    [Serializable]
    public abstract class BlockChainQnaSubmit
    {

    }

    [Serializable]
    public class BlockChainSubmitQuestion : BlockChainQnaSubmit
    {
        public long QuestionId { get; private set; }
        public decimal Price { get; private set; }
        public string UserAddress { get; private set; }

        public BlockChainSubmitQuestion(long questionId, decimal price, string userAddress)
        {
            QuestionId = questionId;
            Price = price;
            UserAddress = userAddress;
        }
    }

    [Serializable]
    public class BlockChainSubmitAnswer : BlockChainQnaSubmit
    {
        public long QuestionId { get; private set; }
        public Guid AnswerId { get; private set; }
        public string SubmiterAddress { get; private set; }


        public BlockChainSubmitAnswer(long questionId, Guid answerId, string submiterAddress)
        {
            QuestionId = questionId;
            AnswerId = answerId;
            SubmiterAddress = submiterAddress;
        }

        public BlockChainSubmitAnswer(long questionId, Guid answerId)
        {
            QuestionId = questionId;
            AnswerId = answerId;
        }
    }


    [Serializable]
    public class BlockChainMarkQuestionAsCorrect : BlockChainQnaSubmit
    {
        public string UserAddress { get; private set; }
        public string WinnerAddress { get; private set; }
        public long QuestionId { get; private set; }
        public Guid AnswerId { get; private set; }


        public BlockChainMarkQuestionAsCorrect(string userAddress, string winnerAddress, long questionId, Guid answerId)
        {
            UserAddress = userAddress;
            WinnerAddress = winnerAddress;
            QuestionId = questionId;
            AnswerId = answerId;
        }
    }

    [Serializable]
    public class BlockChainUpVote : BlockChainQnaSubmit
    {
        public string UserAddress { get; private set; }
        public long QuestionId { get; private set; }
        public Guid AnswerId { get; private set; }


        public BlockChainUpVote(string userAddress, long questionId, Guid answerId)
        {
            UserAddress = userAddress;
            QuestionId = questionId;
            AnswerId = answerId;
        }
    }

}
