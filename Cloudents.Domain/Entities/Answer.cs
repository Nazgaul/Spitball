using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cloudents.Domain.Enums;
using Cloudents.Domain.Interfaces;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]

namespace Cloudents.Domain.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Answer :  ItemObject
    {
        public Answer(Question question, string text, int attachments, RegularUser user) : this()
        {
            Question = question;
            Text = text;
            Attachments = attachments;
            User = user;
            Created = DateTime.UtcNow;
            Item.State = ItemState.Ok;
            // Item.State = Privileges.GetItemState(user.Score);

        }

        protected Answer()
        {
            Item = new ItemComponent();
        }

        public virtual Guid Id { get; set; }
        public virtual Question Question { get; set; }

        public virtual string Text { get; set; }
        public virtual int Attachments { get; set; }
        public virtual RegularUser User { get; set; }

        public virtual DateTime Created { get; set; }

        protected internal virtual IList<Transaction> Transactions { get; set; }

        public virtual IList<Transaction> TransactionsReadOnly => new ReadOnlyCollection<Transaction>(Transactions);
        public override void DeleteAssociation()
        {
            Item.Votes.Clear();
        }
    }

}