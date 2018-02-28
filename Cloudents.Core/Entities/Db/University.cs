using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Attributes;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class University
    {
        public University(long id ,string name)
        {
            Id = id;
            Name = name;
        }

        [UsedImplicitly]
        protected University()
        {

        }

        public virtual long Id { get; set; }

        public virtual bool IsDeleted { get; set; }

        [DbColumn("UniversityName")]
        public virtual string Name { get; set; }
        public virtual string Extra { get; set; }
        public virtual string ExtraSearch { get; set; }
    }
}
