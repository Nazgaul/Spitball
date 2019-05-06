﻿using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "nHibernate Proxy")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "nHibernate Proxy")]
    public abstract class Transaction : Entity<Guid>
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected Transaction()
        {
            Created = DateTime.UtcNow;
        }

        public virtual RegularUser User { get; set; }

        public virtual DateTime Created { get; protected set; }

        public virtual TransactionActionType Action { get; protected set; }
        public virtual TransactionType Type { get; protected set; }
        public virtual decimal Price { get; protected set; }

        //public abstract int AwardScore { get; }

    }

    public class CashOutTransaction : Transaction
    {
        public virtual bool? Approved { get; set; }
        public virtual string DeclinedReason { get; set; }
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public CashOutTransaction() 
        {
            //if (price < 1000)
            //{
            //    throw new ArgumentException();
            //}
            //if (price > 4000)
            //{
            //    throw new ArgumentException();
            //}

            //if (price % 1000 != 0)
            //{
            //    throw new ArgumentException();

            //}
            //price = -Math.Abs(price);

            Price = -1000;
            Action = TransactionActionType.CashOut;
            Type = TransactionType.Spent;
        }

        //protected CashOutTransaction()
        //{

        //}

        public virtual void Approve()
        {
            Approved = true;
        }


        public virtual void Decline(string reason)
        {
            Approved = false;
            DeclinedReason = reason;
        }

        //public override int AwardScore => 0;
    }

    public class CommissionTransaction : Transaction
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public CommissionTransaction(decimal price)
        {
            price = -Math.Abs(price);

            Price = decimal.Round(price * 0.09M, 2);
            Action = TransactionActionType.Commission;
            Type = TransactionType.Spent;
        }

        protected CommissionTransaction()
        {

        }

        //public override int AwardScore => 0;
    }

    public class BuyPointsTransaction : Transaction
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public BuyPointsTransaction(decimal price, string transactionId)
        {
            price = Math.Abs(price);
            TransactionId = transactionId;
            Price = price;
            Action = TransactionActionType.Buy;
            Type = TransactionType.Earned;
        }

        public virtual string TransactionId { get; protected set; }

        protected BuyPointsTransaction()
        {

        }

        //public override int AwardScore => 0;
    }

    public class AwardMoneyTransaction : Transaction
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public AwardMoneyTransaction(decimal price) 
        {
            //if (price < 0)
            //{
            //    throw new ArgumentException("you need to award user");
            //}

           Price = price;
           Action = TransactionActionType.None;
           Type = TransactionType.Earned;
        }

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public AwardMoneyTransaction(AwardsTransaction transaction) : this(transaction.Price)
        {
            Action = transaction.Action;
        }

        protected AwardMoneyTransaction()
        {

        }

        private static readonly SortedSet<string> Tier1Users =
            new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                //"US", "CA", "AU" , "GB", "IE",
                "IL",
                //"NZ", "MX", "SE" ,
                //"NO", "DK", "FI", "NL", "BE","LU","DE","CH","AT","ZA"
            };

        public static AwardMoneyTransaction FinishRegistration(RegularUser user)
        {
            var initBalance = 0;
           //var awardScore = 0;
            if (Tier1Users.Contains(user.Country))
            {
                initBalance = 150;
                //awardScore = 2;
            }
            //Score
            return new AwardMoneyTransaction(initBalance)
            {
                Action = TransactionActionType.SignUp,
              //  _awardScore = awardScore
            };
        }

       // private int _awardScore;
       // public override int AwardScore => _awardScore;
    }

    public class QuestionTransaction : Transaction
    {
        //[SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        //private QuestionTransaction(Question question) 
        //{
        //    Question = question;


        //}

        //private int _awardScore;

        public virtual Question Question { get;  set; }
        public virtual Answer Answer { get; protected set; }

        protected QuestionTransaction()
        {

        }

        //public static QuestionTransaction Asked(Question question)
        //{
        //    var money = -Math.Abs(question.Price);
        //    return new QuestionTransaction(question)
        //    {
        //        Action = TransactionActionType.Question,
        //        Type = TransactionType.Stake,
        //        Price = money
        //    };
        //}

        //public static QuestionTransaction Deleted(Question question)
        //{
        //    var money = Math.Abs(question.Price);
        //    return new QuestionTransaction(question)
        //    {
        //        Question = null,
        //        Action = TransactionActionType.DeleteQuestion,
        //        Type = TransactionType.Stake,
        //        Price = money
        //    };
        //}

        public static void Answered(Question question)
        {
            var userQuestion = question.User;
            var correctAnswer = question.CorrectAnswer;

            //var t1 = new QuestionTransaction(question)
            //{
            //    Action = TransactionActionType.AnswerCorrect,
            //    Type = TransactionType.Stake,
            //    Price = money,
            //    Answer = correctAnswer
            //};
            //var t2 = new QuestionTransaction(question)
            //{
            //    Action = TransactionActionType.AnswerCorrect,
            //    Type = TransactionType.Spent,
            //    Price = -money,
            //    Answer = correctAnswer
            //};
            var t3 = new AwardMoneyTransaction(AwardsTransaction.QuestionOwnerBonus);
            userQuestion.MakeTransaction(t3);

            var userAnswer = correctAnswer.User;

            //This will trigger the correct answer email.

            //var ta1 = new QuestionTransaction(question)
            //{
            //    Action = TransactionActionType.AnswerCorrect,
            //    Type = TransactionType.Earned,
            //    Price = money,
            //    Answer = correctAnswer,
            //    //_awardScore = 1
            //};
            var ta2 = new AwardMoneyTransaction(AwardsTransaction.QuestionAnswererBonus);
            userAnswer.MakeTransaction(ta2);
        }

    }

    public class ReferUserTransaction : Transaction
    {
        public virtual RegularUser InvitedUser { get; protected set; }
      
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public ReferUserTransaction(RegularUser invitedUser) 
        {
            InvitedUser = invitedUser;
            Action = TransactionActionType.ReferringUser;
            Price = 10;
            Type = TransactionType.Earned;
        }

        protected ReferUserTransaction()
        {

        }

       // public override int AwardScore => 0;
    }

    public class DocumentTransaction : Transaction
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        private DocumentTransaction(Document document)
        {
            Document = document;


        }

        protected DocumentTransaction()
        {

        }

        public virtual Document Document { get; protected set; }

        private static Transaction Buyer(Document document)
        {
            return new DocumentTransaction(document)
            {
                Action = TransactionActionType.PurchaseDocument,
                Price = -document.Price,
                Type = TransactionType.Spent
            };
        }

        private static Transaction Seller(Document document)
        {
            return new DocumentTransaction(document)
            {
                Action = TransactionActionType.SoldDocument,
                Price = document.Price,
                Type = TransactionType.Earned,
               // _awardScore = 1

            };
        }

        public static void MakerTransaction(User buyer, User seller, Document d)
        {
            buyer.MakeTransaction(Buyer(d));
            seller.MakeTransaction(Seller(d));
            seller.MakeTransaction(new CommissionTransaction(d.Price));
        }

        //private int _awardScore ;

        //public override int AwardScore => _awardScore;
    }

    public sealed class AwardsTransaction
    {
        private AwardsTransaction(TransactionActionType type, decimal price)
        {
            Action = type;
            Price = price;
        }
        public TransactionActionType Action { get; }
        public decimal Price { get; }

        //public static readonly AwardsTransaction FirstCourse = new AwardsTransaction(TransactionActionType.FirstCourse, 5);
        //public static readonly AwardsTransaction University = new AwardsTransaction(TransactionActionType.Awarded, 5);

        public static readonly AwardsTransaction QuestionOwnerBonus = new AwardsTransaction(TransactionActionType.Awarded, 1);
        public static readonly AwardsTransaction QuestionAnswererBonus = new AwardsTransaction(TransactionActionType.Awarded, 10);
    }


}