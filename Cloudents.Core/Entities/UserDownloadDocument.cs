using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate")]
    public class UserDownloadDocument : IEquatable<UserDownloadDocument>
    {
        public UserDownloadDocument(BaseUser user, Document document)
        {
            if (user.Id == document.User.Id)
            {
                throw new ArgumentException();
            }
            User = user;
            Document = document;
            Created = DateTime.UtcNow;
        }
        protected UserDownloadDocument()
        { }
        public virtual Guid Id { get; }
        public virtual BaseUser User { get; protected set; }
        public virtual Document Document { get; protected set; }
        public virtual DateTime Created { get; }


        public virtual bool Equals(UserDownloadDocument other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(User.Id, other.User.Id) && Equals(Document.Id, other.Document.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UserDownloadDocument)obj);
        }

        public override int GetHashCode()
        {
            var t = (User.Id.GetHashCode() * 53) ^ (Document.Id.GetHashCode() * 37);
            return t;
        }
    }
}
