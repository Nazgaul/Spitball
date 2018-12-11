using System.Globalization;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cloudents.Core.Exceptions;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Question : DomainObject
    {
        private ItemState _state;

        public Question(QuestionSubject subject, string text, decimal price, int attachments,
            RegularUser user,
            QuestionColor color, CultureInfo language)
        : this()
        {
            Subject = subject;
            Text = text?.Trim();
            Price = price;
            Attachments = attachments;
            User = user;
            Updated = Created = DateTime.UtcNow;
            if (color != QuestionColor.Default)
            {
                Color = color;
            }
            State = ReputationSystem.GetItemState(user.Score);
            Language = language;
        }

        public Question(QuestionSubject subject, string text, decimal price, int attachments,
            SystemUser user,
            QuestionColor color, CultureInfo language)
            : this()
        {
            Subject = subject;
            Text = text?.Trim();
            Price = price;
            Attachments = attachments;
            User = user;
            Updated = Created = DateTime.UtcNow;
            if (color != QuestionColor.Default)
            {
                Color = color;
            }

            State = ItemState.Pending;
            Language = language;
        }

        [UsedImplicitly]
        protected Question()
        {
            Answers = Answers ?? new List<Answer>();
        }

        public virtual long Id { get; protected set; }
        public virtual QuestionSubject Subject { get; protected set; }
        public virtual string Text { get; protected set; }
        public virtual decimal Price { get; protected set; }

        public virtual int Attachments { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual DateTime Created { get; protected set; }
        public virtual DateTime Updated { get; set; }

        [CanBeNull]
        public virtual Answer CorrectAnswer { get; set; }

        public virtual IList<Answer> Answers { get; protected set; }


        protected internal virtual IList<Transaction> Transactions { get; set; }

        public virtual QuestionColor? Color { get; set; }

        public virtual ItemState State
        {
            get => _state;
            set => _state = value;
            //{
            //    if (State == ItemState.Ok)
            //    {
            //        var t = Transaction.QuestionApproved(this);
            //        User.AddTransaction(t);
            //        _state = value;
            //    }
            //}
        }

        public virtual Answer AddAnswer(string text, int attachments, RegularUser user)
        {
            var answer = new Answer(this, text, attachments, user);
            Answers.Add(answer);
            return answer;
        }


        [CanBeNull]
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
                throw new InvalidOperationException("Cannot change language of question");
            }

            Language = info;
        }

    }

    //public class QuestionApproved : Question, ISoftDelete
    //{
    //    //public void DeleteAssociation()
    //    //{
    //    //    throw new NotImplementedException();
    //    //}
    //}

    //public class QuestionPending : Question, ISoftDelete
    //{
    //    //public void DeleteAssociation()
    //    //{
    //    //    throw new NotImplementedException();
    //    //}
    //}

    //public class QuestionDeleted : Question
    //{

    //}
}