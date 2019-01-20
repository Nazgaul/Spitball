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
        public RegularUser(string email, string name, string privateKey, Language language) : this()
        {
            Email = email;
            Name = name;
            TwoFactorEnabled = true;
            Language = language;
            PrivateKey = privateKey;
            Created = DateTime.UtcNow;
            //Fictive = false;



        }
        protected RegularUser()
        {
            UserLogins = new List<UserLogin>();
            //Transactions = new List<Transaction>();
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

        private readonly IList<Answer> _answers = new List<Answer>();

        public virtual IReadOnlyList<Answer> Answers => _answers.ToList();
        protected internal virtual IList<UserLogin> UserLogins { get; protected set; }

        public virtual ISet<Course> Courses { get; protected set; }
        public virtual ISet<Tag> Tags { get; protected set; }

        //public virtual int Score { get; set; }

        //public virtual decimal Balance { get; set; }

        //[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "We need internal to do the mapping")]
        // public virtual IList<Transaction> Transactions { get; protected set; }

        public virtual UserTransactions Transactions { get; protected set; }


        //private readonly IList<Transaction> _transactions = new List<Transaction>();
        //public virtual IReadOnlyList<Transaction> Transactions => _transactions.ToList();

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
            Transactions.UpdateBalance(balance, score);
            //AddEvent(new UpdateBalanceEvent(this));
        }





        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(EmailConfirmed)}: {EmailConfirmed}, {nameof(PhoneNumberConfirmed)}: {PhoneNumberConfirmed}";
        }



        public override void MakeTransaction(TransactionType2 transaction, Question question = null,
            Document document = null)
        {
            MakeTransaction(transaction, question, document, null);

        }

        protected virtual void MakeTransaction(TransactionType2 transaction, Question question,
            Document document, RegularUser user)
        {
            var t = new Transaction(transaction, this)
            {
                Question = question,
                Document = document,
                InvitedUser = user
            };
            Transactions.Add(t);
            AddEvent(new TransactionEvent(t, this));
        }

        public virtual void AwardMoney(decimal price)
        {
            MakeTransaction(TransactionType2.AwardToken(price));
        }

        public virtual void CashOutMoney(decimal price)
        {
            MakeTransaction(TransactionType2.CashOut(price));
            //AddEvent(new RedeemEvent(Id, price));
        }

        public virtual void ReferUser(RegularUser user)
        {
            MakeTransaction(TransactionType2.ReferUser,null,null, user);
        }



        //public virtual void Stake(Question question)
        //{
        //    var price = -question.Price;
        //    if (price >= 0)
        //    {
        //        throw new ArgumentException();
        //    }

        //    var t = new Transaction(TransactionType2.StakeMoney(price), this)
        //    {
        //        Question = question
        //    };
        //    Transactions.Add(t);
        //    AddEvent(new TransactionEvent(t));
        //}

        //public virtual void UnStake(Question question)
        //{
        //    var price = question.Price;
        //    if (price <= 0)
        //    {
        //        throw new ArgumentException();
        //    }

        //    var reason = TransactionActionType.DeleteQuestion;
        //    if (question.CorrectAnswer != null)
        //    {
        //        reason = TransactionActionType.AnswerCorrect;
        //    }
        //    var t = new Transaction(TransactionType2.UnStakeMoney(price, reason), this)
        //    {
        //        Question = question
        //    };
        //    Transactions.Add(t);
        //    AddEvent(new TransactionEvent(t));
        //}


        public override int Score { get; protected set; }  //=> Transactions.Score;
        public override decimal Balance => Transactions.Balance;
    }


}