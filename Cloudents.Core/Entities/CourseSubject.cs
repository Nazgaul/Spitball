using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    public class CourseSubject : Entity<long>, IAggregateRoot
    {
        public CourseSubject(string name, Country country)
        {
            Name = name;
            Country = country;
        }
        [SuppressMessage("ReSharper", "CS8618",Justification = "Nhibernate proxy")]
        protected CourseSubject()
        {
        }
        public virtual string Name { get; protected set; }

        public virtual Country Country { get; protected set; }

        public virtual byte[] Version { get; protected set; }

        public virtual void ChangeName(string newName)
        {
            Name = newName;
        }
    }
}
