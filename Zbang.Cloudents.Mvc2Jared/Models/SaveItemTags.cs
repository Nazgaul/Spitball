using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc2Jared.Models
{
    public class SaveItemTags
    {
        public long ItemId { get; set; }
        public long BoxId { get; set; }
        public string ItemName { get; set; }
        public int DocType { get; set; }
        public IEnumerable<string> NewTags { get; set; }
        public IEnumerable<string> RemoveTags { get; set; }
    }
}