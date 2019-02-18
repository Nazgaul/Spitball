using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserUpVotsDto
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public string ItemText { get; set; }
        public char ItemType { get; set; }
    }
}
