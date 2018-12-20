using Cloudents.Common;
using Cloudents.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Cloudents.Application.Enum;
using Cloudents.Common.Enum;

[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]

namespace Cloudents.Domain.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Question : ItemObject
    {
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
            Item.State = Privileges.GetItemState(user.Score);
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

            Item.State = ItemState.Pending;
            Language = language;
        }

        protected Question()
        {
            Answers = Answers ?? new List<Answer>();
            Item = new ItemComponent();
        }

        public virtual long Id { get; protected set; }
        public virtual QuestionSubject Subject { get; protected set; }
        public virtual string Text { get; protected set; }
        public virtual decimal Price { get; protected set; }

        public virtual int Attachments { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual DateTime Created { get; protected set; }
        public virtual DateTime Updated { get; set; }

        public virtual Answer CorrectAnswer { get; set; }

        public virtual IList<Answer> Answers { get; protected set; }


        public virtual IList<Transaction> Transactions { get; protected set; }

        public virtual QuestionColor? Color { get; set; }

        public virtual int AnswerCount { get; set; }

        public virtual Answer AddAnswer(string text, int attachments, RegularUser user)
        {
            var answer = new Answer(this, text, attachments, user);
            Answers.Add(answer);
            return answer;
        }


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
}