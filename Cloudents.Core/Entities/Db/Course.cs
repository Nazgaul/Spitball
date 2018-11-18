using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Course
    {
        public const int MinLength = 4;
        public const int MaxLength = 150;
        protected Course()
        {

        }

        public Course(string name)
        {
            Name = name.Trim();
            if (Name.Length > MaxLength || Name.Length < MinLength)
            {
                throw new ArgumentException();
            }
        }


        public virtual string Name { get; protected set; }
        public virtual int Count { get; set; }
    }
}
