using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Request
{
    public class SeoQuery
    {
        public int Page { get; private set; }

        public Action<SiteMapSeoDto> Callback { get; private set; }
    }
}
