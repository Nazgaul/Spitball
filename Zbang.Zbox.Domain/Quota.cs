using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class Quota
    {
        //Ctor
        public Quota()
        {

        }

        //Properties
        public virtual long TotalSize { get; set; }
        public virtual long UsedSpace { get; set; }
        public long FreeSpace 
        {
            get
            {
                return TotalSize - UsedSpace;
            }
        }

        //Methods
        public void ConsumeSpace(long bytes)
        {
            UsedSpace += bytes;
        }
        
        public void ReleaseSpace(long bytes)
        {
            UsedSpace -= bytes;
        }
    }    
}
