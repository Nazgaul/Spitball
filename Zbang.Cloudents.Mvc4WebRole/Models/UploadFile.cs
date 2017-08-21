using System;
using System.Collections.Generic;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class UploadFile
    {
        public long BoxId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }

        public Guid? TabId { get; set; }

        public bool Comment { get; set; }
    }

    public class UploadChatFile
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }

        public IList<long> Users  { get; set; }
    }
}