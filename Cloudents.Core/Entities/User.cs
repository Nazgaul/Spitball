﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

//[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate proxy")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public abstract class User : AggregateRoot
    {


        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        protected User()
        {
            
           
         
        }

        //public virtual long Id { get; set; }
       

        public virtual string Name { get; set; }
        public virtual string NormalizedName { get; set; }
        public virtual string NormalizedEmail { get; set; }
        public virtual string SecurityStamp { get; set; }

        public virtual string Image { get; set; }

        //TODO: should fictive user need to have university
        public virtual University University { get;  set; }

     

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual string AuthenticatorKey { get; set; }

        public virtual string PrivateKey { get; set; }
        public virtual bool? OldUser { get; set; }

        //public virtual int Score { get; protected set; }


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


      

        private readonly IList<Question> _questions = new List<Question>();
        public virtual IReadOnlyList<Question> Questions => _questions.ToList();


      


        public virtual DateTime Created { get; protected set; }

        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }

        public virtual bool Fictive { get; protected set; }



       
        

        //public abstract IList<Transaction> Transactions { get; set; }
        public abstract decimal Balance { get; }

        public abstract int Score { get; protected set; }

        //public abstract void AddTransaction(Transaction t);

        public abstract void MakeTransaction(TransactionType2 transaction, Question question = null, Document document = null);



        public virtual CultureInfo Culture { get; set; }

        public virtual string Country { get; set; }

        protected virtual void DeleteQuestion()
        {
            _questions.Clear();
        }

    }
}
