namespace Cloudents.Core.Entities
{
    public class Tutor : UserRole
    {
        public Tutor(string bio, decimal price)
        {
            Bio = bio;
            Price = price;
        }

        protected Tutor()
        {
            
        }
        public virtual string Bio { get; set; }
        public virtual decimal Price { get; set; }
    }
}