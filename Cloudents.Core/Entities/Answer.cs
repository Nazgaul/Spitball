using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]
[assembly: InternalsVisibleTo("Cloudents.Persistance")]

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Answer : ItemObject
    {
        public Answer(Question question, string text, int attachments, RegularUser user)
            : this()
        {
            Question = question;
            Text = text;
            Attachments = attachments;
            User = user;
            Created = DateTime.UtcNow;
            MakePublic();

        }

        protected Answer()
        {
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
            Votes.Clear();
        }


        //public virtual void DeleteAnswer()
        //{
        //    Events.Add(new AnswerDeletedEvent(this));
        //}

        public override bool Delete()
        {
            var t = base.Delete();
            if (t)
            {
                Events.Add(new AnswerDeletedEvent(this));
            }

            return t;
        }

        public virtual void DeleteAnswerAdmin()
        {
            Transactions.Clear();
            Events.Add(new AnswerDeletedEvent(this));
            if (Question.CorrectAnswer != null)
            {
                if (Id == Question.CorrectAnswer.Id)
                {
                    Question.CorrectAnswer = null;
                }
            }
        }

        public override bool MakePublic()
        {
            var t =  base.MakePublic();
            if (t)
            {
                Events.Add(new AnswerCreatedEvent(this));
            }

            return t;
        }

        //public override ItemComponent Item { get; set; }
    }

}