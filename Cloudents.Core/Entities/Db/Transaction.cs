using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "nHibernate Proxy")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "nHibernate Proxy")]
    public class Transaction : IEvents
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
        private Transaction(ActionType action, TransactionType type, decimal price) : this()
        {
            Action = action;
            Type = type;
            Price = price;
            Created = DateTime.UtcNow;

           
        }

        [UsedImplicitly]
        protected Transaction()
        {
            Events = new List<IEvent>();
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

        public static Transaction DistributeTokens(ActionType actionType, TransactionType transactionType, decimal money)
        {
            return new Transaction(actionType, transactionType, money);
        }

        public static Transaction ReferringUserTransaction(User invitedUser)
        {
            var Tx =  new Transaction(ActionType.ReferringUser, TransactionType.Earned, 10)
            {
                InvitedUser = invitedUser
                
        };
            Tx.Events.Add(new ReferredUserEvent(Tx));
            return Tx;
        }

        public static Transaction QuestionDelete(Question question)
        {
            return new Transaction(ActionType.DeleteQuestion, TransactionType.Stake, question.Price);

        }

        public static Transaction CashOut(decimal price)
        {
            //if (price > 0)
            //{
            //    throw new InvalidOperationException("amount should be negative");
            //}
            price = -Math.Abs(price);
            return new Transaction(ActionType.CashOut, TransactionType.Earned, price);
        }

        //[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Nhibernate proxy")]
        //protected Transaction()
        //{

        //}

        public virtual Guid Id { get; protected set; }
        public virtual User User { get; set; }

        public virtual DateTime Created { get; protected set; }

        public virtual ActionType Action { get; protected set; }
        public virtual TransactionType Type { get; protected set; }
        public virtual decimal Price { get; protected set; }

        [CanBeNull] public virtual Question Question { get; set; }
        [CanBeNull] public virtual Answer Answer { get; protected set; }
        [CanBeNull] public virtual User InvitedUser { get; protected set; }
        public virtual IList<IEvent> Events { get; }
    }
}