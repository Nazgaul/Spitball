using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]
    public class RegularUser : User
    {
        public RegularUser(string email, string name,  Language language) : this()
        {
            Email = email;
            Name = name;
            TwoFactorEnabled = true;
            Language = language;
            Created = DateTime.UtcNow;
            //Fictive = false;



        }
        protected RegularUser()
        {
            UserLogins = new List<UserLogin>();
            Transactions = Transactions ?? new UserTransactions();
            Courses = new HashSet<Course>();
            Tags = new HashSet<Tag>();

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

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual string LockoutReason { get; set; }

        // ReSharper disable once CollectionNeverUpdated.Local Nhiberate
        private readonly IList<Answer> _answers = new List<Answer>();

        public virtual IReadOnlyList<Answer> Answers => _answers.ToList();
        protected internal virtual IList<UserLogin> UserLogins { get; protected set; }

        public virtual ISet<Course> Courses { get; protected set; }
        public virtual ISet<Tag> Tags { get; protected set; }

        public virtual DateTime LastOnline { get; protected set; }
        public virtual bool Online { get; protected set; }

        public virtual UserTransactions Transactions { get; protected set; }

        public virtual string FirstName { get; protected set; }
        public virtual string LastName { get; protected set; }
        public virtual string Description { get; protected set; }

        public virtual void ChangeOnlineStatus(bool isOnline)
        {
            Online = isOnline;
            LastOnline = DateTime.UtcNow;

        }

        public virtual void SuspendUser(DateTimeOffset lockTime, string reason)
        {
            LockoutEnd = lockTime;
            LockoutReason = reason;
            AddEvent(new UserSuspendEvent(this));
        }

        public virtual void UnSuspendUser()
        {
            LockoutEnd = DateTime.UtcNow.Add(new TimeSpan(0,0,-1));
            AddEvent(new UserUnSuspendEvent(this));
        }

        public virtual void UpdateUserBalance(decimal balance, int score)
        {
            Transactions.UpdateBalance(balance, score);
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(EmailConfirmed)}: {EmailConfirmed}, {nameof(PhoneNumberConfirmed)}: {PhoneNumberConfirmed}";
        }

        public override void MakeTransaction(Transaction transaction)
        {
            Transactions.Add(transaction, this);
            AddEvent(new TransactionEvent(transaction, this));

        }


        public virtual void AwardMoney(decimal price)
        {
            var t = new AwardMoneyTransaction(price);
            MakeTransaction(t);
            
        }

        public virtual void AwardMoney(AwardsTransaction award)
        {
            var t = new AwardMoneyTransaction(award);
            MakeTransaction(t);
        }

        public virtual void CashOutMoney(/*decimal price*/)
        {
            var t = new CashOutTransaction();
            MakeTransaction(t);
        }

        public virtual void ReferUser(RegularUser user)
        {
            MakeTransaction(new ReferUserTransaction(user));
        }

        public virtual void FinishRegistration()
        {
            MakeTransaction(AwardMoneyTransaction.FinishRegistration(this));
        }

        public virtual void ConfirmePhoneNumber()
        {
            if (PhoneNumberConfirmed == false)
            {
                PhoneNumberConfirmed = true;
            }
        }

        public override int Score { get; protected set; }  //=> Transactions.Score;
        public override decimal Balance => Transactions.Balance;
    }


}