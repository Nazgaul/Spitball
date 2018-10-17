using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Answer : IEvents
    {
        public Answer(Question question, string text, int attachments, User user) : this()
        {
            Question = question;
            Text = text;
            Attachments = attachments;
            User = user;
            Created = DateTime.UtcNow;

            Events.Add(new AnswerCreatedEvent(this));
        }

        [UsedImplicitly]
        protected Answer()
        {
            Events = new List<IEvent>();
        }

        public virtual Guid Id { get; set; }
        public virtual Question Question { get; set; }

        public virtual string Text { get; set; }
        public virtual int Attachments { get; set; }
        public virtual User User { get; set; }

        public virtual DateTime Created { get; set; }

        protected internal virtual Question QuestionAnswerCorrect { get; set; }
       
        public virtual IList<IEvent> Events { get; }
    }
}