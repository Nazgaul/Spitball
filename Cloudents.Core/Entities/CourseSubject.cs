namespace Cloudents.Core.Entities
{
    public class CourseSubject : Entity<int>, IAggregateRoot
    {

        protected CourseSubject()
        {
        }
        public virtual string Name { get; set; }

        public virtual byte[] Version { get; protected set; }
    }
}
