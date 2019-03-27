using System.Collections.Generic;
using System.Linq;

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
        private readonly ISet<TutorsCourses> _courses = new HashSet<TutorsCourses>();
        public virtual IReadOnlyCollection<TutorsCourses> Courses => _courses.ToList();
        public const string RoleName = "Tutor";
    }
}