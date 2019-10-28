namespace Cloudents.Core.Entities
{
    public class CourseSubject : Entity<long>, IAggregateRoot
    {

        protected CourseSubject()
        {
        }
        public virtual string Name { get; set; }

        public virtual byte[] Version { get; protected set; }
    }
}
