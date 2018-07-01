using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Answer
    {
        public Answer(Question question, string text, int attachments, User user)
        {
            Question = question;
            Text = text;
            Attachments = attachments;
            User = user;
            Created = DateTime.UtcNow;
            //AnswerCreateTransaction();
        }

        [UsedImplicitly]
        protected Answer()
        {
        }

        public virtual Guid Id { get; set; }
        public virtual Question Question { get; set; }

        public virtual string Text { get; set; }
        public virtual int Attachments { get; set; }
        public virtual User User { get; set; }

        public virtual DateTime Created { get; set; }

        public virtual Transaction Transaction { get; set; }

        //public virtual Transaction AnswerCreateTransaction()
        //{
        //    var t = new Transaction(ActionType.Answer, TransactionType.Pending, Question.Price);
        //    User.AddTransaction(t);
        //    return t;
        //}

        //public virtual void AnswerDeleteTransaction()
        //{
        //    var t =  new Transaction(ActionType.Answer, TransactionType.Pending, -Question.Price);
        //    User.AddTransaction(t);
        //}
    }
}