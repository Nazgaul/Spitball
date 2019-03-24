namespace Cloudents.Core.Entities
{
    public class Tutor : UserRole
    {
        public virtual string Bio { get; set; }
        public virtual decimal Price { get; set; }
    }
}