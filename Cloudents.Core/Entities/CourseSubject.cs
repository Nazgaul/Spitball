using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Entities
{
    public class CourseSubject: AggregateRoot<long>
    {
        public const int MinLength = 4;
        public const int MaxLength = 150;
        public CourseSubject(string name)
        {
            var TrimedName = name.Trim();
            if (TrimedName.Length > MaxLength || TrimedName.Length < MinLength)
            {
                throw new ArgumentException($"Name is {TrimedName}", nameof(TrimedName));
            }
            Name = TrimedName;
        }
        protected CourseSubject() { }
        public virtual string Name { get; set; }
    }
}
