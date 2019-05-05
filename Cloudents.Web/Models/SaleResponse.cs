using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class SaleResponse
    {
        public SaleResponse(Uri link)
        {
            Link = link;
        }

        public Uri Link { get;  }
    }
}
