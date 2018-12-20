using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Domain.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]

    public class Vote
    {
        public Vote(RegularUser user, Document document, VoteType voteType) : this()
        {
            User = user;
            Document = document;
            VoteType = voteType;
        }

        public Vote(RegularUser user, Answer answer, VoteType voteType) : this()
        {
            User = user;
            Answer = answer;
            Question = answer.Question;
            VoteType = voteType;
        }

        public Vote(RegularUser user, Question question, VoteType voteType) : this()
        {
            User = user;
            Question = question;
            VoteType = voteType;
        }

        protected Vote()
        {
            TimeStamp = new DomainTimeStamp();
        }

        public virtual Guid Id { get; set; }
        public virtual RegularUser User { get; set; }
        public virtual Document Document { get; set; }
        public virtual Answer Answer { get; set; }
        public virtual Question Question { get; set; }
        public virtual DomainTimeStamp TimeStamp { get; set; }
        public virtual VoteType VoteType { get; set; }
    }

    public enum VoteType : int
    {
        Down = -1,
        None = 0,
        Up = 1,

    }
}