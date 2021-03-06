﻿using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class SuspendedUsersDto
    {
        public long UserId { get; set; }
        public string UserEmail { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
