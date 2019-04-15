namespace Cloudents.Core.Entities
{
    public class CourseSubject: AggregateRoot<int>
    {

        protected CourseSubject()
        {
        }
        public virtual string Name { get; set; }
    }
}
