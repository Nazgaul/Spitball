using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.DTOs.Emails
{
    public class BadItemDto
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public long Uid { get; set; }
        public string ItemName { get; set; }
        public long BoxUid { get; set; }
    }
}
