using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.DTOs.Emails
{
    public class UserDigestDto
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string Culture { get; set; }
    }
}
