using Cloudents.Core.Enum;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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

        public Answer(Question question, string text, User user, CultureInfo language)
            : this()
        {
            Question = question;
            if (text != null && (text.Length > MaxLength || text.Length < MinLength))
            {
                throw new ArgumentException();
            }
            Text = text;
            User = user;
            Created = DateTime.UtcNow;
            Language = language;
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

        //  protected internal virtual IList<Transaction> Transactions { get; set; }

        //public virtual IList<Transaction> TransactionsReadOnly => new ReadOnlyCollection<Transaction>(Transactions);


        public virtual ItemStatus Status { get; set; }




        //private readonly ICollection<Vote> _votes = new List<Vote>();
        //public virtual IReadOnlyCollection<Vote> Votes => _votes.ToList();

        // public virtual int VoteCount { get;  set; }

        public virtual CultureInfo Language { get; protected set; }


        public virtual void UnFlag()
        {
            if (Status.State != ItemState.Flagged) return;

            //if (Status.FlagReason?.Equals(TooManyVotesReason, StringComparison.CurrentCultureIgnoreCase) == true)
            //{
            //    _votes.Clear();
            //    VoteCount = 0;
            //}
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

        //public virtual void Vote(VoteType type, User user)
        //{
        //    if (Status != Public)
        //    {
        //        throw new NotFoundException();
        //    }
        //    if (User == user)
        //    {
        //        throw new UnauthorizedAccessException("you cannot vote you own answer");
        //    }

        //    if (Question.Answers.Any(w => w.Status == Public && w.User == user))
        //    {
        //        throw new UnauthorizedAccessException("you cannot vote if you gave answer");
        //    }
        //    var vote = Votes.FirstOrDefault(w => w.User == user);
        //    if (vote == null)
        //    {
        //        vote = new Vote(user, this, type);
        //        _votes.Add(vote);

        //    }

        //    vote.VoteType = type;
        //    VoteCount = Votes.Sum(s => (int) s.VoteType);
        //    if (VoteCount < VoteCountToFlag)
        //    {
        //        Status = Status.Flag(TooManyVotesReason, user);
        //    }
        //}

        public virtual void Delete()
        {
            //_votes.Clear();
            Status = ItemStatus.Delete();
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

    }

}