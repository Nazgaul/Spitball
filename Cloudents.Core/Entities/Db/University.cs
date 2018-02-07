using System;

namespace Cloudents.Core.Entities.Db
{
    public class University : IDirty
    {
        protected University()
        {
        }

        public virtual long Id { get; set; }

        public virtual bool IsDeleted { get; set; }
        public void DeleteAssociation()
        {
        }

        public virtual bool IsDirty { get; set; }
        public virtual Func<bool> ShouldMakeDirty => () => false;
    }
}
