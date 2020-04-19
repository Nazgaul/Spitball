//using System;
//using System.Diagnostics.CodeAnalysis;

//namespace Cloudents.Core.Entities
//{
//    public class DocumentCourse : Entity<Guid>, IEquatable<DocumentCourse>
//    {
//        protected DocumentCourse()
//        {

//        }

//        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
//        public DocumentCourse(Document document, Course2 course)
//        {
//            Document = document;
//            Course = course;
//        }

//        public virtual Document Document { get; protected set; }
//        public virtual Course2 Course { get; protected set; }




//        public virtual bool Equals(DocumentCourse? other)
//        {
//            if (ReferenceEquals(null, other)) return false;
//            if (ReferenceEquals(this, other)) return true;
//            return Document.Id.Equals(other.Document.Id) && Course.Id.Equals(other.Course.Id);
//        }

//        public override bool Equals(object? obj)
//        {
//            if (ReferenceEquals(null, obj)) return false;
//            if (ReferenceEquals(this, obj)) return true;
//            if (obj.GetType() != this.GetType()) return false;
//            return Equals((DocumentCourse)obj);
//        }

//        public override int GetHashCode()
//        {
//            return HashCode.Combine(491,Document.Id, Course.Id);
//        }
//    }
//}