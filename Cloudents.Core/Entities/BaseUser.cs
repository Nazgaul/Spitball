﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

//[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate proxy")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public abstract class BaseUser : Entity<long>, IAggregateRoot
    {


        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        protected BaseUser()
        {



        }



        public virtual string Name { get; protected set; }
        //public virtual string NormalizedEmail { get; set; }
        public virtual string? SecurityStamp { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global reserved for later use
        [Obsolete]
        public virtual string Image { get; protected set; }
        public virtual string? ImageName { get; protected set; }

        public virtual string AuthenticatorKey { get; set; }




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




        // ReSharper disable once CollectionNeverUpdated.Local Nhibernate
        private readonly ICollection<Question> _questions = new List<Question>();
        public virtual IEnumerable<Question> Questions => _questions;


        private readonly ICollection<Document> _documents = new List<Document>();
        public virtual IEnumerable<Document> Documents => _documents;


        public virtual DateTime Created { get; protected set; }

        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global Nhibernate
        public virtual bool Fictive { get; protected set; }

        public abstract decimal Balance { get; }

        //public abstract int Score { get; protected set; }

        public abstract void MakeTransaction(Transaction transaction);

        public virtual CultureInfo Language { get; protected set; }

        public virtual void ChangeLanguage(Language lang)
        {
            Language = lang;
        }

        public virtual string Country { get; protected set; }
        public virtual Country SbCountry { get; protected set; }

        public virtual byte[] Version { get; protected set; }

       


        public abstract void AddFollower(User follower);


//        public abstract void RemoveFollower(BaseUser follower);
       
    }
}
