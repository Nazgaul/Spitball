using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]
[assembly: InternalsVisibleTo("Cloudents.Persistance")]

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Answer : Entity<Guid>, ISoftDelete
    {
        public Answer(Question question, string text, int attachments, RegularUser user, CultureInfo language)
            : this()
        {
            Question = question;
            Text = text;
            Attachments = attachments;
            User = user;
            Created = DateTime.UtcNow;
            //MakePublic();
            Language = language;
            State = ItemState2.Public();

        }

        protected Answer()
        {
        }

       // public virtual Guid Id { get; set; }
        public virtual Question Question { get; set; }

        public virtual string Text { get; set; }
        public virtual int Attachments { get; set; }
        public virtual RegularUser User { get; set; }

        public virtual DateTime Created { get; set; }

        protected internal virtual IList<Transaction> Transactions { get; set; }

        public virtual IList<Transaction> TransactionsReadOnly => new ReadOnlyCollection<Transaction>(Transactions);


        public virtual ItemState2 State { get; set; }

       


        public virtual ICollection<Vote> Votes { get; protected set; }

        public virtual int VoteCount { get;  set; }

        public virtual CultureInfo Language { get; protected set; }
        //for dbi only
        public virtual void SetLanguage(CultureInfo info)
        {
            if (info.Equals(CultureInfo.InvariantCulture))
            {
                return;
            }

            if (Language != null)
            {
                throw new InvalidOperationException("Cannot change language of answer");
            }

            Language = info;
        }

        public virtual void UnFlag()
        {
            if (State.FlagReason.Equals(ItemState2.TooManyVotesReason, StringComparison.CurrentCultureIgnoreCase))
            {
                Votes.Clear();
                VoteCount = 0;
            }
            State = ItemState2.Public();
           
        }

        public virtual void Delete()
        {
            if (State == ItemState2.Delete())
            {
                return;
            }
            Votes.Clear();
            State = ItemState2.Delete();
           
        }

        //public virtual void DeleteAnswerAdmin()
        //{
        //    Transactions.Clear();
        //   // AddEvent(new AnswerDeletedEvent(this));
        //    if (Question.CorrectAnswer != null)
        //    {
        //        if (Id == Question.CorrectAnswer.Id)
        //        {
        //            Question.CorrectAnswer = null;
        //        }
        //    }
        //}

        //public override bool MakePublic()
        //{
        //    var t = base.MakePublic();
        //    AddEvent(new AnswerCreatedEvent(this));

        //    return t;
        //}

        //public override void ChangeState(ItemState state)
        //{
        //    base.ChangeState(state);
        //    if (State == ItemState.Pending)
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(state), state, null);
        //    }
        //}
        public virtual void  Flag(string messageFlagReason, User user)
        {
            State = State.Flag(messageFlagReason, user);
        }
    }

}