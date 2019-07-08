namespace Cloudents.Core.Entities
{
    public class ViewUserDetail
    {

        protected ViewUserDetail()
        {
            
        }

        public virtual  long Id { get; protected set; }
        public virtual  string Name { get; protected set; }
        public virtual  string Image { get; protected set; }
        public virtual  string Courses { get; protected set; }
        public virtual decimal Price { get; protected set; }
        public virtual string Bio { get; protected set; }
        public virtual float? Rate { get; protected set; }

        public virtual int Score { get; set; }

        public virtual int ReviewsCount { get; protected set; }
        public virtual bool IsTutor { get; protected set; }
    }
}