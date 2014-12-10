using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class UploadFile
    {
        public long BoxId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }

        public Guid? TabId { get; set; }

        public bool Question { get; set; }

    }
}