using System.Collections.Generic;

namespace Cloudents.Core.Entities
{
    public class ViewTutor
    {
        protected ViewTutor()
        {
            
        }

        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Image { get; set; }
        public virtual IEnumerable<string> Subjects { get; set; }
        public virtual IEnumerable<string> Courses { get; set; }
        public virtual int CourseCount { get; set; }
        public virtual decimal Price { get; set; }
        public virtual float Rate { get; set; }
        public virtual int SumRate { get; set; }
        public virtual string Bio { get; set; }
        public virtual string University { get; set; }
        public virtual int Lessons { get; set; }
    }
}