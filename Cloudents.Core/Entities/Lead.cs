using System;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities
{
    public class Lead :Entity<Guid>
    {
        public Lead(Course course, string text, [CanBeNull] University university, string referer,
            [CanBeNull] User user, string name, string phone, string email, Tutor tutor, string utmSource)
        {
            Course = course;
            Text = text;
            University = university;
            User = user;
            Referer = referer;
            Name = name;
            Phone = phone;
            Email = email;
            Tutor = tutor;
            UtmSource = utmSource;
            CreationTime = DateTime.UtcNow;
        }

      
        protected Lead()
        {
        }

        [CanBeNull]
        public virtual User User { get;protected set; }
        public virtual Course Course { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual string Phone { get; protected set; }

        public virtual string  Email { get; protected set; }
        public virtual string Text { get; protected set; }
        [CanBeNull]
        public virtual University University { get; protected set; }
        public virtual string Referer { get; protected set; }


        [CanBeNull]
        public virtual Tutor Tutor { get; protected set; }

        public virtual string UtmSource { get;protected set; }

        public virtual DateTime CreationTime { get; set; }
    }
}