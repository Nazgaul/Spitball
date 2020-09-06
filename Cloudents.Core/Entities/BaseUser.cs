namespace Cloudents.Core.Entities
{
    public abstract class BaseUser : Entity<long> , IAggregateRoot
    {
        protected BaseUser()
        {
            
        }
        public virtual byte[] Version { get; protected set; }
        public virtual string Email { get; set; }
        public virtual string Name { get; protected set; }
        public virtual string FirstName { get; protected set; }
        public virtual string? ImageName { get; protected set; }


    }
}