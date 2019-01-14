using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

using static Cloudents.Core.Entities.ItemState2;
[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Question : AggregateRoot, ISoftDelete
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

            State = GetInitState(user);
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
            State = Pending();
            //ChangeState(ItemState.Pending);
            Language = language;
        }

        protected Question()
        {
            Answers = Answers ?? new List<Answer>();
            Votes = Votes ?? new List<Vote>();
        }

        public virtual ItemState2 State { get; protected set; }

        //public virtual long Id { get; protected set; }
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

        // public virtual int AnswerCount { get; set; }

        public virtual Answer AddAnswer(string text, int attachments, RegularUser user, CultureInfo language)
        {
            var answer = new Answer(this, text, attachments, user, language);
            Answers.Add(answer);
            AddEvent(new AnswerCreatedEvent(answer));
            return answer;
        }

        public virtual void RemoveAnswer(Answer answer, bool admin = false)
        {
            Answers.Remove(answer);
            if (admin)
            {
                Transactions.Clear();
                AddEvent(new AnswerDeletedEvent(answer));
                if (CorrectAnswer != null)
                {
                    if (answer == CorrectAnswer)
                    {
                        CorrectAnswer = null;
                    }
                }
            }
        }

        


        public virtual CultureInfo Language { get; protected set; }

        public virtual ICollection<Vote> Votes { get; protected set; }

        public virtual int VoteCount { get; set; }


        public virtual void MakePublic()
        {
            State = Public();
            AddEvent(new QuestionCreatedEvent(this));
        }

        public virtual void DeleteQuestionAdmin()
        {
            Transactions.Clear();
            AddEvent(new QuestionDeletedAdminEvent(this));
        }

        public virtual void Delete()
        {
            Votes.Clear();
            //Transactions.Clear();
            AddEvent(new QuestionDeletedEvent(this));

        }

        public virtual void AcceptAnswer(Answer answer)
        {
            if (CorrectAnswer != null)
            {
                throw new InvalidOperationException("Already have correct answer");
            }

            CorrectAnswer = answer;
            AddEvent(new MarkAsCorrectEvent(answer));
        }

        public virtual void Flag(string messageFlagReason, User user)
        {
            State = State.Flag(messageFlagReason, user);
        }
    }
}