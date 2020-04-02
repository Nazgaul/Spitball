using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static Cloudents.Core.Entities.ItemStatus;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]
[assembly: InternalsVisibleTo("Cloudents.Persistence")]

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Answer : Entity<Guid>, ISoftDelete
    {

        public const int MinLength = 15;
        public const int MaxLength = 550;

        public Answer(Question question, string text, User user)
            : this()
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Question = question;
            if (text.Length > MaxLength || text.Length < MinLength)
            {
                throw new ArgumentException();
            }
            Text = text;
            User = user;
            Created = DateTime.UtcNow;
            Status = Public;

        }

        protected Answer()
        {
        }

        // public virtual Guid Id { get; set; }
        public virtual Question Question { get; set; }

        public virtual string Text { get; set; }
        public virtual User User { get; set; }

        public virtual DateTime Created { get; set; }

        public virtual ItemStatus Status { get; set; }

        protected internal virtual ICollection<QuestionTransaction> Transaction { get; set; }
        public virtual void UnFlag()
        {
            if (Status.State != ItemState.Flagged) return;
            Status = Public;
        }
        public virtual void Flag(string messageFlagReason, User user)
        {
            if (User == user)
            {
                throw new UnauthorizedAccessException("you cannot flag your own question");
            }
            Status = Status.Flag(messageFlagReason, user);
        }



        public virtual void Delete()
        {
            //_votes.Clear();
            Transaction.Clear();
            Status = ItemStatus.Delete();
        }
    }

}