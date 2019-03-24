namespace Cloudents.Core.Entities
{
    public class Tutor : UserRole
    {
        public Tutor(string bio, decimal price,RegularUser user) :base(user)
        {
            Bio = bio;
            Price = price;
        }

        protected Tutor()
        {
            
        }
        public virtual string Bio { get; set; }
        public virtual decimal Price { get; set; }
        public override string Name => RoleName;

        public const string RoleName = "Tutor";
    }
}