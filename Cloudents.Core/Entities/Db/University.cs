using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class University 
    {
        public University(long id ,string name)
        {
            Id = id;
            Name = name;
        }

        protected University()
        {
            
        }

        public virtual long Id { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual string Name { get; set; }
        public virtual string Extra { get; set; }
        public virtual string ExtraSearch { get; set; }
    }
}
