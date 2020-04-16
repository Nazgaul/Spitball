using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Entities
{
    public class Course2 : Entity<long>
    {
        public Course2(Country country, string field, string subject, string searchDisplay, string cardDisplay)
        {
            Country = country;
            Field = field;
            Subject = subject;
            SearchDisplay = searchDisplay;
            CardDisplay = cardDisplay;
            Created = DateTime.UtcNow;


        }


        public Course2(Country country,  string searchDisplay)
        {
            Country = country;
            //Field = field;
            //Subject = subject;
            SearchDisplay = searchDisplay;
            //CardDisplay = cardDispaly;
            Created = DateTime.UtcNow;
            State = ItemState.Pending;


        }

        protected Course2()
        {

        }
        public virtual Country Country { get; protected set; }
        public virtual string Field { get; protected set; }
        public virtual string Subject { get; protected set; }
        public virtual string SearchDisplay { get; protected set; }
        public virtual string CardDisplay { get; protected set; }

        public virtual DateTime Created { get; protected set; }

        public virtual int Count { get; protected internal set; }

        public virtual ItemState State { get; protected set; }

        private readonly ISet<UserCourse2> mUsers = new HashSet<UserCourse2>();

        public virtual IEnumerable<UserCourse2> Users => mUsers;

        public virtual void Approve()
        {
            //TODO: maybe put an event to that
            if (State == ItemState.Pending)
            {
                State = ItemState.Ok;
            }
        }
    }
}
