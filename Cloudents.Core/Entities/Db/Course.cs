using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Course
    {
        protected Course()
        {

        }

        public Course(string name)
        {
            Name = name.Trim();
            if (Name.Length > 150 || Name.Length < 4)
            {
                throw new ArgumentException();
            }
        }


        public virtual string Name { get; protected set; }
        public virtual int Count { get; set; }
    }
}
