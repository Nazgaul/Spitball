using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

//[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate proxy")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public abstract class User : Entity<long>, IAggregateRoot
    {


        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        protected User()
        {
            
           
         
        }

        //public virtual long Id { get; set; }
       

        public virtual string Name { get; protected set; }
       // public virtual string NormalizedName { get; set; }
        public virtual string NormalizedEmail { get; set; }
        public virtual string SecurityStamp { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global reserved for later use
        public virtual string Image { get; set; }

        //TODO: should fictive user need to have university
        public virtual University University { get; protected  set; }


        public virtual void SetUniversity(University university)
        {
            University = university;
            University.UsersCount++;
        }

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual string AuthenticatorKey { get; set; }

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


      

        // ReSharper disable once CollectionNeverUpdated.Local Nhibernate
        private readonly IList<Question> _questions = new List<Question>();
        public virtual IReadOnlyList<Question> Questions => _questions.ToList();


      


        public virtual DateTime Created { get; protected set; }

        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global Nhibernate
        public virtual bool Fictive { get; protected set; }

        public abstract decimal Balance { get; }

        public abstract int Score { get; protected set; }

        public abstract void MakeTransaction(Transaction transaction);



        public virtual CultureInfo Language { get;protected set; }

        public virtual void ChangeLanguage(Language lang)
        {
            Language = lang;
        }

        public virtual string Country { get; set; }

        protected virtual void DeleteQuestion()
        {
            _questions.Clear();
        }
    }
}
