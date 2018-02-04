using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public abstract class ItemCommentBase
    {
        public virtual long Id { get; set; }
        public virtual Item Item { get; set; }
        
        public virtual string Comment { get; set; }

        public virtual UserTimeDetails UserTime { get; set; }

        public virtual User Author { get; set; }

        protected virtual ICollection<Updates> Updates { get; set; }
    }
}