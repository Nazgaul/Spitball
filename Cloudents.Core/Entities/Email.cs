using System.Collections.Generic;

namespace Cloudents.Core.Entities
{
    public class Email : Entity<int>
    {
        public virtual string Subject { get; set; }
       // public virtual EmailBlock EmailBlock1 { get; set; }
      //  public virtual EmailBlock EmailBlock2 { get; set; }
        public virtual bool SocialShare { get; set; }
        public virtual SystemEvent Event { get; set; }
        public virtual Language Language { get; set; }

        public virtual IList<EmailBlock> EmailBlock { get; set; }

    }
}