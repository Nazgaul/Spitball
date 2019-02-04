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


        protected Transaction()
        {
            Created = DateTime.UtcNow;
            ;
        }

        public virtual RegularUser User { get; set; }

        public virtual DateTime Created { get; protected set; }

        public virtual TransactionActionType Action { get; protected set; }
        public virtual TransactionType Type { get; protected set; }
        public virtual decimal Price { get; protected set; }





        //public virtual TransactionType2 TransactionType { get; protected set; }
    }

    public class CashOutTransaction : Transaction
    {

        public CashOutTransaction(decimal price/*, RegularUser user*/) //: base(user)
        {
            if (price < 1000)
            {
                throw new ArgumentException();
            }
            if (price > 4000)
            {
                throw new ArgumentException();
            }

            if (price % 1000 != 0)
            {
                throw new ArgumentException();

            }
            price = -Math.Abs(price);

            this.Price = price;
            this.Action = TransactionActionType.CashOut;
            this.Type = Enum.TransactionType.Spent;
        }

        protected CashOutTransaction()
        {

        }
    }

    public class CommissionTransaction : Transaction
    {
        public CommissionTransaction(decimal price)
        {
            price = -Math.Abs(price);

            this.Price = decimal.Round(price * 0.09M, 2);
            this.Action = TransactionActionType.Commission;
            this.Type = Enum.TransactionType.Spent;
        }

        protected CommissionTransaction()
        {

        }
    }

    public class BuyPointsTransaction : Transaction
    {
        public BuyPointsTransaction(decimal price, string transactionId)
        {
            price = Math.Abs(price);
            TransactionId = transactionId;
            this.Price = price;
            this.Action = TransactionActionType.Buy;
            this.Type = Enum.TransactionType.Earned;
        }

        public virtual string TransactionId { get; set; }

        protected BuyPointsTransaction()
        {

        }
    }

    public class AwardMoneyTransaction : Transaction
    {
        public AwardMoneyTransaction(decimal price/*, RegularUser user*/) //: base(user)
        {
            if (price < 0)
            {
                throw new ArgumentException("you need to award user");
            }

            this.Price = price;
            this.Action = TransactionActionType.None;
            this.Type = Enum.TransactionType.Earned;
        }

        public AwardMoneyTransaction(AwardsTransaction transaction/*, RegularUser user*/) : this(transaction.Price/*, user*/)
        {
            Action = transaction.Action;
        }

        protected AwardMoneyTransaction()
        {

        }

        private static readonly SortedSet<string> Tier1Users =
            new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "US", "CA", "AU" , "GB", "IE", "IL", "NZ", "MX", "SE" ,
                "NO", "DK", "FI", "NL", "BE","LU","DE","CH","AT","ZA"
            };

        public static AwardMoneyTransaction FinishRegistration(RegularUser user)
        {
            var initBalance = 100;
            if (Tier1Users.Contains(user.Country))
            {
                initBalance = 150;
            }

            return new AwardMoneyTransaction(initBalance/*, user*/)
            {
                Action = TransactionActionType.SignUp
            };
        }

    }

    public class QuestionTransaction : Transaction
    {
        private QuestionTransaction(Question question/*, RegularUser user*/) //: base(user)
        {
            Question = question;


        }

        public virtual Question Question { get; set; }
        public virtual Answer Answer { get; set; }

        protected QuestionTransaction()
        {

        }

        public static QuestionTransaction Asked(Question question)
        {
            var money = -Math.Abs(question.Price);
            return new QuestionTransaction(question/*, user*/)
            {
                Action = TransactionActionType.Question,
                Type = TransactionType.Stake,
                Price = money
            };
        }

        public static QuestionTransaction Deleted(Question question)
        {
            var money = Math.Abs(question.Price);
            return new QuestionTransaction(question)
            {
                Action = TransactionActionType.DeleteQuestion,
                Type = TransactionType.Stake,
                Price = money
            };
        }

        public static void Answered(Question question)
        {
            var money = Math.Abs(question.Price);
            var userQuestion = question.User;
            var correctAnswer = question.CorrectAnswer;

            var t1 = new QuestionTransaction(question)
            {
                Action = TransactionActionType.AnswerCorrect,
                Type = TransactionType.Stake,
                Price = money,
                Answer = correctAnswer
            };
            var t2 = new QuestionTransaction(question)
            {
                Action = TransactionActionType.AnswerCorrect,
                Type = TransactionType.Spent,
                Price = -money,
                Answer = correctAnswer
            };
            var t3 = new AwardMoneyTransaction(AwardsTransaction.QuestionOwnerBonus);
            userQuestion.MakeTransaction(t1);
            userQuestion.MakeTransaction(t2);
            userQuestion.MakeTransaction(t3);



            var userAnswer = correctAnswer.User;
            var ta1 = new QuestionTransaction(question/*, user*/)
            {
                Action = TransactionActionType.AnswerCorrect,
                Type = TransactionType.Earned,
                Price = money,
                Answer = correctAnswer
            };
            var ta2 = new AwardMoneyTransaction(AwardsTransaction.QuestionAnswererBonus);
            userAnswer.MakeTransaction(ta1);
            userAnswer.MakeTransaction(ta2);
            userAnswer.MakeTransaction(new CommissionTransaction(question.Price));
        }

    }

    public class ReferUserTransaction : Transaction
    {
        public virtual RegularUser InvitedUser { get; set; }
        //    public static readonly TransactionType2 ReferUser = new TransactionType2(
        // TransactionActionType.ReferringUser, TransactionType.Earned, 10);
        public ReferUserTransaction(RegularUser invitedUser/*, RegularUser user*/) //: base(user)
        {
            InvitedUser = invitedUser;
            Action = TransactionActionType.ReferringUser;
            this.Price = 10;
            this.Type = TransactionType.Earned;
        }

        protected ReferUserTransaction()
        {

        }
    }

    public class DocumentTransaction : Transaction
    {
        private DocumentTransaction(Document document/*, RegularUser user*/) //: base(user)
        {
            Document = document;


        }

        protected DocumentTransaction()
        {

        }

        public virtual Document Document { get; set; }

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
                Type = TransactionType.Earned
            };
        }

        public static void MakerTransaction(User buyer, User seller, Document d)
        {
            var price = d.Price;
            buyer.MakeTransaction(Buyer(d));
            seller.MakeTransaction(Seller(d));
            seller.MakeTransaction(new CommissionTransaction(d.Price));
        }

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

        public static readonly AwardsTransaction FirstCourse = new AwardsTransaction(TransactionActionType.FirstCourse, 5);
        public static readonly AwardsTransaction University = new AwardsTransaction(TransactionActionType.Awarded, 5);

        public static readonly AwardsTransaction QuestionOwnerBonus = new AwardsTransaction(TransactionActionType.Awarded, 1);
        public static readonly AwardsTransaction QuestionAnswererBonus = new AwardsTransaction(TransactionActionType.Awarded, 10);
    }


}