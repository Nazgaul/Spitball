using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cloudents.Core.Enum;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure.Data")]
namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate proxy")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public class User
    {
        public User(string email, string name, string privateKey) : this()
        {
            Email = email;
            Name = name;
            TwoFactorEnabled = true;
            PrivateKey = privateKey;
            UserCreateTransaction();
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        protected User()
        {
            Transactions = new List<Transaction>();
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

        public virtual void AddTransaction(Transaction t)
        {
            t.User = this;
            t.Balance = (LastTransaction?.Balance ?? 0) + t.Price;
            Transactions.Add(t);
            LastTransaction = t;
        }

        private const decimal InitialBalance = 100;

        public virtual Transaction UserCreateTransaction()
        {
            var t =  new Transaction(ActionType.SignUp, TransactionType.Awarded, InitialBalance);
            AddTransaction(t);
            return t;
        }

        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "We need internal to do the mapping")]
        protected internal virtual Transaction LastTransaction { get; set; }

        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "We need internal to do the mapping")]
        protected internal virtual IList<Transaction> Transactions { get; set; }
    }
}
