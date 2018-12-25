using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]
[assembly: InternalsVisibleTo("Cloudents.Persistance")]

namespace Cloudents.Core.Entities
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
            ChangeState(Privileges.GetItemState(user.Score));
            
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

        public virtual void ChangeState(ItemState state)
        {
            Item.ChangeState(state);
            }
        public override void DeleteAssociation()
        {
            Item.Votes.Clear();
        }
        }

        public virtual void DeleteAnswer()
        {

        }

        public virtual void DeleteAnswerAdmin()
        {
            this.Transactions.Clear();
            this.Events.Add(new AnswerDeletedEvent(this));
            if (Question.CorrectAnswer != null)
            {
                if (Id == Question.CorrectAnswer.Id)
                {
                    Question.CorrectAnswer = null;
                }
            }
        }
        //public override ItemComponent Item { get; set; }
    }

}