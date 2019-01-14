using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]
    public class RegularUser : User
    {
        public RegularUser(string email, string name, string privateKey, CultureInfo culture)
        {
            Email = email;
            Name = name;
            TwoFactorEnabled = true;
            Culture = culture;
            PrivateKey = privateKey;
            Created = DateTime.UtcNow;
            //Fictive = false;



        }
        protected RegularUser()
        {
            UserLogins = new List<UserLogin>();

        }

        public virtual int FraudScore { get; set; }


        public virtual void DeleteQuestionAndAnswers()
        {
            DeleteQuestion();
            _answers.Clear();
        }
       
        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        public virtual int AccessFailedCount { get; set; }

        public virtual bool LockoutEnabled { get; set; }

        private readonly IList<Answer> _answers = new List<Answer>();

        public virtual IReadOnlyList<Answer> Answers => _answers.ToList();
        protected internal virtual IList<UserLogin> UserLogins { get; protected set; }


        public virtual void SuspendUser(DateTimeOffset lockTime)
        {
            LockoutEnd = lockTime;
            AddEvent(new UserSuspendEvent(this));
        }

        public virtual void UnSuspendUser()
        {
            LockoutEnd = null;
            AddEvent(new UserUnSuspendEvent(this));
        }

        public virtual void UpdateUserBalance(decimal balance, int score)
        {
            Balance = balance;
            Score = score;

            AddEvent(new UpdateBalanceEvent(this));
        }

        public virtual void CashOutMoney(decimal price)
        {
            price = -Math.Abs(price);
            var t = new Transaction(TransactionActionType.CashOut, TransactionType.Earned, price, this);
            AddEvent(new RedeemEvent(Id, price));
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(EmailConfirmed)}: {EmailConfirmed}, {nameof(PhoneNumberConfirmed)}: {PhoneNumberConfirmed}";
        }
    }
}