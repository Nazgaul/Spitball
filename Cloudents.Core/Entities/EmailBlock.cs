namespace Cloudents.Core.Entities
{
    public class EmailBlock : Entity<int>
    {
        public virtual string Title { get; protected set; }
        public virtual string SubTitle { get; protected set; }
        public virtual string MinorTitle { get; protected set; }
        public virtual string Body { get; protected set; }
        public virtual string Cta { get; protected set; }
        public virtual int Order { get; protected set; }
    }
}