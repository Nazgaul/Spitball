using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models.FAQ
{
    public class QnA
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Order { get; set; }
    }
}