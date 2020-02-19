using System;

namespace Cloudents.Core.Entities
{
    public class AdminTutor
    {
        public AdminTutor(Tutor tutor)
        {
            Tutor = tutor;
        }
        protected AdminTutor()
        { }

        public virtual Guid Id { get; protected set; }
        public virtual Tutor Tutor { get; protected set; }
    }
}
