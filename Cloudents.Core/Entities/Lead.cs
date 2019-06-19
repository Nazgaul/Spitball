using System;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities
{
    public class Lead :Entity<Guid>
    {
        public Lead(Course course, string text, [CanBeNull] University university, [CanBeNull] User user, string utmSource)
        {
            Course = course;
            Text = text;
            University = university;
            User = user;
            UtmSource = utmSource;
        }

        public Lead(Course course, string name, string phone, string text, [CanBeNull] University university, string utmSource)
        {
            Course = course;
            Name = name;
            Phone = phone;
            Text = text;
            University = university;
            UtmSource = utmSource;
        }

        protected Lead()
        {
        }

        [CanBeNull]
        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
        public virtual string Name { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Text { get; set; }
        [CanBeNull]
        public virtual University University { get; set; }
        public virtual string UtmSource { get; set; }
    }
}