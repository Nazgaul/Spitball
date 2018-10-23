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
        }


        public virtual string Name { get; protected set; }
        public virtual int Count { get; set; }
    }
}
