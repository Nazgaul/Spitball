using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using static Cloudents.Core.Entities.ItemStatus;
using static Cloudents.Core.Entities.Vote;

//[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Question : AggregateRoot, ISoftDelete
    {
        public Question(QuestionSubject? subject, string text, decimal price, int attachments,
            RegularUser user,
             CultureInfo language, [NotNull] Course course, [NotNull] University university)
        : this()
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            if (university == null) throw new ArgumentNullException(nameof(university));
            Subject = subject;
            Text = text?.Trim();
            Price = price;
            Attachments = attachments;
            User = user;
            Updated = Created = DateTime.UtcNow;


            var status = GetInitState(user);
            if (status == Public)
            {
                MakePublic();
            }

            Status = status;
            Course = course;
            University = university;
            Language = language ?? new CultureInfo("en");
        }

        public Question(Course course, string text, decimal price, int attachments,
            SystemUser user,
             CultureInfo language, University university)
            : this()
        {
            Course = course;
            Text = text?.Trim();
            Price = price;
            Attachments = attachments;
            User = user;
            Updated = Created = DateTime.UtcNow;

            Status = Pending;
            //ChangeState(ItemState.Pending);
            Language = language ?? new CultureInfo("en");
            University = university;

        }

        protected Question()
        {
            _answers = _answers ?? new List<Answer>();
            _votes = _votes ?? new List<Vote>();
        }

        public virtual ItemStatus Status { get; protected set; }

        //public virtual long Id { get; protected set; }
        public virtual QuestionSubject? Subject { get; protected set; }
        public virtual string Text { get; protected set; }
        public virtual decimal Price { get; protected set; }

        public virtual int Attachments { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual DateTime Created { get; protected set; }
        public virtual DateTime Updated { get; set; }

        public virtual Course Course { get; set; }
        public virtual University University { get; set; }

        public virtual Answer CorrectAnswer { get; set; }

        private readonly IList<Answer> _answers = new List<Answer>();

        public virtual IReadOnlyList<Answer> Answers => _answers.ToList();


        public virtual IList<QuestionTransaction> Transactions { get; protected set; }


        // public virtual int AnswerCount { get; set; }

        public virtual Answer AddAnswer(string text, int attachments, RegularUser user, CultureInfo language)
        {
            var answer = new Answer(this, text, attachments, user, language);
            _answers.Add(answer);
            AddEvent(new AnswerCreatedEvent(answer));
            return answer;
        }

        public virtual void RemoveAnswer(Answer answer, bool admin = false)
        {
            _answers.Remove(answer);
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

        public virtual void Vote(VoteType type, RegularUser user)
        {
            if (Status != Public)
            {
                throw new NotFoundException();
            }
            if (User == user)
            {
                throw new UnauthorizedAccessException("you cannot vote you own question");
            }
            var vote = Votes.FirstOrDefault(w => w.User == user && w.Answer == null);
            if (vote == null)
            {
                vote = new Vote(user, this, type);
                _votes.Add(vote);

            }

            vote.VoteType = type;
            VoteCount = Votes.Where(w => w.Answer == null).Sum(s => (int)s.VoteType);
            if (VoteCount < VoteCountToFlag)
            {
                Status = Status.Flag(TooManyVotesReason, user);
            }
        }

        [NotNull]
        public virtual CultureInfo Language { get; protected set; }

        private readonly IList<Vote> _votes = new List<Vote>();

        public virtual IReadOnlyCollection<Vote> Votes => _votes.ToList();

        public virtual int VoteCount { get; protected set; }


        public virtual void MakePublic()
        {
            //TODO: maybe put an event to that
            if (Status == null || Status == Pending)
            {
                Status = Public;
                AddEvent(new QuestionCreatedEvent(this));
            }
        }

        public virtual void DeleteQuestionAdmin()
        {
            Delete();
            foreach (var tran in Transactions)
            {
                tran.Question = null;
            }
            AddEvent(new QuestionDeletedAdminEvent(this));
        }

        public virtual void Delete()
        {
            Status = ItemStatus.Delete();
            _votes.Clear();
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
            if (User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot flag your own question");
            }

            Status = Status.Flag(messageFlagReason, user);

        }

        public virtual void UnFlag()
        {
            if (Status.State != ItemState.Flagged)
            {
                throw new ArgumentException();
            }

            if (Status.FlagReason?.Equals(TooManyVotesReason, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                _votes.Clear();
                VoteCount = 0;
            }
            Status = Public;
        }
    }


}