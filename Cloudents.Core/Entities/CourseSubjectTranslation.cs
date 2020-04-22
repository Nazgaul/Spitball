//using System;

//namespace Cloudents.Core.Entities
//{
//    public class CourseSubjectTranslation : Entity<Guid>, IEquatable<CourseSubjectTranslation>
//    {
//        public CourseSubjectTranslation(CourseSubject subject, AdminLanguage language, string nameTranslation)
//        {
//            Subject = subject;
//            Language = language;
//            NameTranslation = nameTranslation;
//        }

//        protected CourseSubjectTranslation() { }

//        public virtual CourseSubject Subject { get; protected set; }
//        public virtual AdminLanguage Language { get; protected set; }
//        public virtual string NameTranslation { get; protected set; }

//        public virtual void ChangeName(string name)
//        {
//            NameTranslation = name;
//        }

//        public virtual bool Equals(CourseSubjectTranslation other)
//        {
//            if (ReferenceEquals(null, other)) return false;
//            if (ReferenceEquals(this, other)) return true;
//            return Equals(Subject.Id, other.Subject.Id) && Equals(Language.Id, other.Language.Id);
//        }

//        public override bool Equals(object obj)
//        {
//            if (ReferenceEquals(null, obj)) return false;
//            if (ReferenceEquals(this, obj)) return true;
//            if (obj.GetType() != GetType()) return false;
//            return Equals((CourseSubjectTranslation)obj);
//        }

//        public override int GetHashCode()
//        {
//            var t = (Subject.Id.GetHashCode() * 29) ^ (Language.Id.GetHashCode() * 47);
//            return t;
//        }
//    }
//}
