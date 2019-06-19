using System;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities
{
    public class Lead :Entity<Guid>
    {
        public Lead(Course course, string text, [CanBeNull] University university, string referer, [CanBeNull] User user, string name, string phone)
        {
            Course = course;
            Text = text;
            University = university;
            User = user;
            Referer = referer;
            Name = name;
            Phone = phone;
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
        public virtual string Referer { get; set; }
    }
}