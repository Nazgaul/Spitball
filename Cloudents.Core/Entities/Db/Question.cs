using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class Question
    {
        public Question(QuestionSubject subject, string text, decimal price, int attachments, User user)
        {
            Subject = subject;
            Text = text;
            Price = price;
            Attachments = attachments;
            User = user;
        }

        protected Question()
        {
        }

        public virtual long Id { get; set; }
        public virtual QuestionSubject Subject { get; set; }
        public virtual string Text { get; set; }
        public virtual decimal Price { get; set; }

        public virtual int Attachments { get; set; }

        public virtual User User { get; set; }
    }
}