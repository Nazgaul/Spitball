using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Answer : DomainObject, ISoftDelete
    {
        public Answer(Question question, string text, int attachments, RegularUser user) : this()
        {
            Question = question;
            Text = text;
            Attachments = attachments;
            User = user;
            Created = DateTime.UtcNow;
            Item.State = Privileges.GetItemState(user.Score);
            if (Item.State == ItemState.Ok)
            {
                Events.Add(new AnswerCreatedEvent(this));
            }
        }

        [UsedImplicitly]
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


        public virtual ItemComponent Item { get; set; }
        //public virtual ItemState State { get; set; }
    }
    
}