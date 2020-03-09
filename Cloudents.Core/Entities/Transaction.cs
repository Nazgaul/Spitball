using Cloudents.Core.Enum;
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

        public virtual User User { get; set; }

        public virtual DateTime Created { get; protected set; }

        public virtual TransactionActionType Action { get; protected set; }
        public virtual TransactionType Type { get; protected set; }
        public virtual decimal Price { get; protected set; }




    }

    public class CashOutTransaction : Transaction
    {
        public virtual bool? Approved { get; set; }
        public virtual string DeclinedReason { get; set; }
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public CashOutTransaction()
        {
            Price = -1000;
            Action = TransactionActionType.CashOut;
            Type = TransactionType.Spent;
        }
        public virtual void Approve()
        {
            Approved = true;
        }


        public virtual void Decline(string reason)
        {
            Approved = false;
            DeclinedReason = reason;
        }
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
    }

    public class BuyPointsTransaction : Transaction
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public BuyPointsTransaction(decimal price, string transactionId, decimal localCurrencyPrice)
        {
            price = Math.Abs(price);
            TransactionId = transactionId;
            Price = price;
            LocalCurrencyPrice = localCurrencyPrice;
            Action = TransactionActionType.Buy;
            Type = TransactionType.Earned;
        }

        public virtual string TransactionId { get; protected set; }
        public virtual decimal LocalCurrencyPrice { get; protected set; }

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
                "IL","US"
                //"NZ", "MX", "SE" ,
                //"NO", "DK", "FI", "NL", "BE","LU","DE","CH","AT","ZA"
            };

        public static AwardMoneyTransaction FinishRegistration(User user)
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
    }
    /// <summary>
    /// Question Transaction - we keep this because of older transactions
    /// </summary>
    public class QuestionTransaction : Transaction
    {


        public virtual Question Question { get; set; }
        public virtual Answer Answer { get; protected set; }

        protected QuestionTransaction()
        {

        }


    }

    public class ReferUserTransaction : Transaction
    {
        public virtual User InvitedUser { get; protected set; }

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public ReferUserTransaction(User invitedUser)
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

        public static void MakerTransaction(BaseUser buyer, BaseUser seller, Document d)
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
    }


}