using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudents.Core.Entities
{
    public class ReadTutor : Entity<long>
    {
        public ReadTutor()
        { }
        
        public virtual string Name { get; protected set; }
        public virtual string Image { get; protected set; }
        public virtual IEnumerable<string> Subjects { get; protected set; }
        public virtual IEnumerable<string>  AllSubjects { get; protected set; }
        public virtual IEnumerable<string> Courses { get; protected set; }
        public virtual IEnumerable<string> AllCourses { get; protected set; }
        public virtual decimal Price { get; protected set; }
        public virtual double Rate { get; protected set; }
        public virtual int RateCount { get; protected set; }
        public virtual string Bio { get; protected set; }
        public virtual string University { get; protected set; }
        public virtual int Lessons { get; protected set; }
        public virtual double Rating { get; protected set; }
    }
}
