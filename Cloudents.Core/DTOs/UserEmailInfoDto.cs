﻿
using System;

namespace Cloudents.Core.DTOs
{
    public class UserEmailInfoDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid University { get; set; }

        public string PhoneNumber { get; set; }
        public int LeadCount { get; set; }
    }
}
