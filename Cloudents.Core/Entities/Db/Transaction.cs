using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{
    public class Transaction
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
        private Transaction(ActionType action, TransactionType type, decimal price)
        {
            Action = action;
            Type = type;
            Price = price;
            Created = DateTime.UtcNow;
        }

        public static Transaction CorrectAnswer(TransactionType type, Question question,
            Answer answer)
        {
            var price = question.Price;
            if (type == TransactionType.Spent)
            {
                price = -price;
            }
            return new Transaction(ActionType.AnswerCorrect, type, price)
            {
                Question = question,
                Answer = answer
            };
        }

        private const decimal InitialBalance = 100;

        public static Transaction UserCreate()
        {
            return new Transaction(ActionType.SignUp, TransactionType.Awarded, InitialBalance);
        }

        public static Transaction QuestionCreate(Question question)
        {
            return new Transaction(ActionType.Question, TransactionType.Stake, -question.Price)
            {
                Question = question
            };
        }

        public static Transaction QuestionDelete(Question question)
        {
            return new Transaction(ActionType.DeleteQuestion, TransactionType.Stake, question.Price);

        }

        public static Transaction CashOut(decimal price)
        {
            if (price > 0)
            {
                throw new InvalidOperationException("amount should be negative");
            }
            return new Transaction(ActionType.CashOut, TransactionType.Earned, -price);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Nhibernate proxy")]
        protected Transaction()
        {

        }

        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }

        public virtual DateTime Created { get; set; }

        public virtual ActionType Action { get; set; }
        public virtual TransactionType Type { get; set; }
        public virtual decimal Price { get; set; }

        public virtual Question Question { get; set; }
        public virtual Answer Answer { get; set; }
    }
}