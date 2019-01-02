using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Cloudents.Core.Message.Email;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate proxy")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public abstract class User  : AggregateRoot
    {


        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        protected User()
        {
            Transactions = new List<Transaction>();
           
            Courses = new HashSet<Course>();
            Tags = new HashSet<Tag>();
        }

        public virtual long Id { get; set; }
       

        public virtual string Name { get; set; }
        public virtual string NormalizedName { get; set; }
        public virtual string NormalizedEmail { get; set; }
        public virtual string SecurityStamp { get; set; }

        public virtual string Image { get; set; }

        public virtual University University { get; set; }

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual string AuthenticatorKey { get; set; }

        public virtual string PrivateKey { get; set; }
        public virtual bool? OldUser { get; set; }

        public virtual int Score { get; set; }

        public virtual object Actual => this;

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
        public virtual IList<Transaction> Transactions { get; protected set; }

        protected readonly IList<Question> _questions = new List<Question>();
        public virtual IReadOnlyList<Question> Questions => _questions.ToList();
       

        


        public virtual ISet<Course> Courses { get; protected set; }
        public virtual ISet<Tag> Tags { get; protected set; }


        public virtual DateTime Created { get; protected set; }

        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }

        public virtual bool Fictive { get; protected set; }

       



        public virtual CultureInfo Culture { get; set; }

        public virtual string Country { get; set; }

    }
}
