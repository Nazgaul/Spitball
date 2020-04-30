using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static Cloudents.Core.Entities.ItemStatus;

//[assembly: InternalsVisibleTo("Cloudents.Infrastructure")]

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Question : Entity<long>, IAggregateRoot, ISoftDelete
    {
        public Question(string text,
            User user,
            [NotNull] Course course)
        : this()
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Text = text.Trim();
            User = user;
            Updated = Created = DateTime.UtcNow;


            var status = GetInitState(user);
            if (status == Public)
            {
                MakePublic();
            }

            Status = status;
            Course = course ?? throw new ArgumentException();
        }

        public Question(Course course, string text, SystemUser user)
            : this()
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Course = course;
            Text = text.Trim();
            User = user;
            Updated = Created = DateTime.UtcNow;

            Status = Pending;

        }

        protected Question()
        {
            _answers ??= new List<Answer>();
            //_votes = _votes ?? new List<Vote>();
        }

        public virtual ItemStatus Status { get; protected set; }

        public virtual string Text { get; protected set; }


        public virtual BaseUser User { get; protected set; }

        public virtual DateTime Created { get; protected set; }
        public virtual DateTime Updated { get; set; }

        [NotNull]
        public virtual Course Course { get; set; }

        private readonly IList<Answer> _answers = new List<Answer>();

        public virtual IReadOnlyList<Answer> Answers => _answers.ToList();

        public virtual Answer AddAnswer(string text, User user)
        {
            var answer = new Answer(this, text, user);
            _answers.Add(answer);
            return answer;
        }

        public virtual void RemoveAnswer(Answer answer)
        {
            _answers.Remove(answer);

        }

        

        //[NotNull]
        //public virtual CultureInfo Language { get; protected set; }

        public virtual void MakePublic()
        {
            //TODO: maybe put an event to that
            if (Status == Pending)
            {
                Status = Public;
                //AddEvent(new QuestionCreatedEvent(this));
            }
        }

        public virtual void DeleteQuestionAdmin()
        {
            Delete();
        }

        public virtual void Delete()
        {
            Status = ItemStatus.Delete();
            _answers.Clear();

        }





        public virtual void Flag(string messageFlagReason, BaseUser user)
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

            //if (Status.FlagReason?.Equals(TooManyVotesReason, StringComparison.CurrentCultureIgnoreCase) == true)
            //{
            //    //_votes.Clear();
            //    //VoteCount = 0;
            //}
            Status = Public;
        }
    }


}