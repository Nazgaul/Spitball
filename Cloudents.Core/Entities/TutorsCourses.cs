using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Entities
{
    public class TutorsCourses : Entity<Guid>
    {

        public TutorsCourses(Tutor tutor, Course course)
        {
            Tutor = tutor;
            Course = course;
        }

        protected TutorsCourses()
        {

        }

        public virtual Tutor Tutor { get; set; }
        public virtual Course Course { get; set; }
    }
}
