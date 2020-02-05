using System;

namespace Cloudents.Core.Entities
{
    public class CourseSubjectTranslation : Entity<Guid>
    {
        public CourseSubjectTranslation(CourseSubject subject, AdminLanguage language, string nameTranslation)
        {
            Subject = subject;
            Language = language;
            NameTranslation = nameTranslation;
        }

        protected CourseSubjectTranslation() { }
        public virtual CourseSubject Subject { get; protected set; }
        public virtual AdminLanguage Language { get; protected set; }
        public virtual string NameTranslation { get; protected set; }
    }
}
