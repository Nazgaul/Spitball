using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "nHibernate Proxy")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "nHibernate Proxy")]
    public class Transaction : Entity<Guid>
    {
        //[SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
        //public Transaction(TransactionActionType action, TransactionType type, decimal price, RegularUser user)
        //    : this()
        //{
        //    Action = action;
        //    Type = type;
        //    Price = price;
        //    Created = DateTime.UtcNow;
        //    User = user;
        //    AddEvent(new TransactionEvent(this));

        //}

        public Transaction(TransactionType2 type, RegularUser user) : this()
        {
            TransactionType = type;
            Created = DateTime.UtcNow;
            User = user;
            //AddEvent(new TransactionEvent(this));
        }

        protected Transaction()
        {

        }

        //  public virtual Guid Id { get; protected set; }
        public virtual RegularUser User { get; protected set; }

        public virtual DateTime Created { get; protected set; }

        // public virtual TransactionActionType Action { get; protected set; }
        // public virtual TransactionType Type { get; protected set; }
        // public virtual decimal Price { get; protected set; }

        public virtual Question Question { get; set; }
        public virtual Answer Answer { get; set; }
        public virtual RegularUser InvitedUser { get; set; }

        public virtual Document Document { get; set; }


        public virtual TransactionType2 TransactionType { get; protected set; }

    }


    public sealed class TransactionType2 : ValueObject
    {
        public TransactionActionType Action { get; }

        public TransactionType Type { get; }

        public decimal Price { get; }

        private TransactionType2()
        {

        }

        private TransactionType2(TransactionActionType action, TransactionType type, decimal amount)
        {
            Action = action;
            Type = type;
            Price = amount;
        }

        public static TransactionType2 CashOut(decimal price)
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
            return new TransactionType2(TransactionActionType.CashOut, TransactionType.Earned, price);
        }

        public static TransactionType2 AwardToken(decimal price)
        {
            if (price < 0)
            {
                throw new ArgumentException("you need to award user");
            }
            return new TransactionType2(TransactionActionType.None, TransactionType.Earned, price);
        }
        public static TransactionType2 StakeMoney(decimal money)
        {
            money = -Math.Abs(money);
            return new TransactionType2(TransactionActionType.Question, TransactionType.Stake, money);
        }

        public static TransactionType2 UnStakeMoney(decimal money, TransactionActionType reason)
        {

            return new TransactionType2(reason, TransactionType.Stake, money);
        }

        public static TransactionType2 Spend(decimal money, TransactionActionType reason)
        {
            money = -Math.Abs(money);
            return new TransactionType2(reason, TransactionType.Spent, money);
        }

        public static TransactionType2 Earn(decimal money, TransactionActionType reason)
        {
            money = Math.Abs(money);
            return new TransactionType2(reason, TransactionType.Earned, money);
        }

        private static readonly SortedSet<string> Tier1Users =
            new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                    "US", "CA", "AU" , "GB", "IE", "IL", "NZ", "MX", "SE" ,
                    "NO", "DK", "FI", "NL", "BE","LU","DE","CH","AT","ZA"
            };
        public static TransactionType2 FinishRegistration(string country)
        {
            var initBalance = 100;
            if (Tier1Users.Contains(country))
            {
                initBalance = 150;
            }
            return new TransactionType2(TransactionActionType.SignUp, TransactionType.Earned, initBalance);

        }

        public static readonly TransactionType2 FirstCourse = new TransactionType2(TransactionActionType.FirstCourse, TransactionType.Earned, 5);
        public static readonly TransactionType2 University = new TransactionType2(TransactionActionType.Awarded, TransactionType.Earned, 5);
        public static readonly TransactionType2 ReferUser = new TransactionType2(TransactionActionType.ReferringUser, TransactionType.Earned, 10);
        public static readonly TransactionType2 QuestionOwnerBonus = new TransactionType2(TransactionActionType.Awarded, TransactionType.Earned, 1);
        public static readonly TransactionType2 QuestionAnswererBonus = new TransactionType2(TransactionActionType.Awarded, TransactionType.Earned, 10);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Action;
            yield return Type;
            yield return Price;
        }
    }
}