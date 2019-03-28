using System.Collections.Generic;

namespace Cloudents.Core.Entities
{
    public class Tutor
    {
        public Tutor(string bio, decimal price, long userId) 
        {
            UserId = userId;
            Bio = bio;
            Price = price;
            //Courses = new HashSet<TutorsCourses>();
        }

        protected Tutor()
        {
            //Courses = new HashSet<TutorsCourses>();
        }
        public virtual string Bio { get; set; }
        public virtual decimal Price { get; set; }
        public virtual long UserId { get; set; }
        /*public override string Name => RoleName;
        //private readonly ISet<TutorsCourses> _courses = new HashSet<TutorsCourses>();
        //public virtual IReadOnlyCollection<TutorsCourses> Courses => _courses.ToList();
        public virtual ISet<TutorsCourses> Courses { get; protected set; }
        public const string RoleName = "Tutor";*/
    }
}