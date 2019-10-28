using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]

    public class Vote : Entity<Guid>
    {
        public const int VoteCountToFlag = -2;
        public Vote(User user, Document document, VoteType voteType) : this()
        {
            User = user;
            Document = document;
            VoteType = voteType;
        }

        //public Vote(User user, Answer answer, VoteType voteType) : this()
        //{
        //    User = user;
        //    Answer = answer;
        //    Question = answer.Question;
        //    VoteType = voteType;
        //}

        //public Vote(User user, Question question, VoteType voteType) : this()
        //{
        //    User = user;
        //    Question = question;
        //    VoteType = voteType;
        //}

        protected Vote()
        {
            TimeStamp = new DomainTimeStamp();
        }

        //public virtual Guid Id { get; set; }
        public virtual User User { get;protected set; }
        public virtual Document Document { get; protected set; }
        //public virtual Answer Answer { get; protected set; }
       // public virtual Question Question { get; protected set; }
        public virtual DomainTimeStamp TimeStamp { get; protected set; }
        public virtual VoteType VoteType { get;  set; }
    }

    public enum VoteType : int
    {
        Down = -1,
        None = 0,
        Up = 1,

    }
}