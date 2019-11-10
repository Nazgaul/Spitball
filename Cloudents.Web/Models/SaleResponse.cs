using System;

namespace Cloudents.Web.Models
{
    public class SaleResponse
    {
        public SaleResponse(Uri link)
        {
            Link = link;
        }

        public Uri Link { get; }
    }
}
