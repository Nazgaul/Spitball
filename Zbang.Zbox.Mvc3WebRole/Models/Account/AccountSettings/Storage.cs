using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Zbox.Mvc3WebRole.Models.Account.AccountSettings
{
    public class Storage
    {
        public long AllocatedSize { get; set; }
        public long UsedSpace { get; set; }

        public long Percent
        {
            get
            {
                if (AllocatedSize == 0)
                {
                    return 0;
                } 
                return UsedSpace * 100 / AllocatedSize;
            }
        }

        public long Free
        {
            get
            {
                return 100 - Percent;
            }
        }
    }
}