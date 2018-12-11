using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]
namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate proxy")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public abstract class User : DomainObject
    {
        

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        protected User()
        {
            Transactions = new List<Transaction>();
            UserLogins = new List<UserLogin>();
            Courses = new HashSet<Course>();
            Tags = new HashSet<Tag>();
        }

        public virtual long Id { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }

        public virtual string Name { get; set; }
        public virtual string NormalizedName { get; set; }
        public virtual string NormalizedEmail { get; set; }
        public virtual string SecurityStamp { get; set; }

        public virtual string Image { get; set; }

        public virtual University University { get; set; }

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual string AuthenticatorKey { get; set; }

        public virtual string PrivateKey { get; set; }
        public virtual int FraudScore { get; set; }
        public virtual bool? OldUser { get; set; }

        public virtual int Score { get; set; }


        /// <summary>
        /// To be reused for (NHibernate) Linq generator
        /// </summary>
        //public static readonly Expression<Func<User, decimal>> CalculateBalanceExpression = x =>
        //    x.Transactions.Count();

        //private static readonly Func<User, decimal> CalculateBalance = CalculateBalanceExpression.Compile();

        //public virtual decimal Fee
        //{
        //    get { return CalculateBalance(this); }
        //}


        public virtual decimal Balance { get; set; }

        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "We need internal to do the mapping")]
        protected internal virtual IList<Transaction> Transactions { get; protected set; }
        protected internal virtual IList<Question> Questions { get; set; }
        protected internal virtual IList<Answer> Answers { get; set; }
        protected internal virtual IList<UserLogin> UserLogins { get; protected set; }


        protected internal virtual ISet<Course> Courses { get; protected set; }
        public virtual ISet<Tag> Tags { get; protected set; }


        public virtual DateTime Created { get; protected set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(EmailConfirmed)}: {EmailConfirmed}, {nameof(PhoneNumberConfirmed)}: {PhoneNumberConfirmed}";
        }

        public virtual bool Fictive { get; protected set; }

        public virtual string PasswordHash { get; set; }
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        public virtual int AccessFailedCount { get; set; }

        public virtual bool LockoutEnabled { get; set; }



        [CanBeNull] public virtual CultureInfo Culture { get; set; }

        public virtual string Country { get; set; }

    }


    public class SystemUser : User
    {

    }


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

        }
    }
}
