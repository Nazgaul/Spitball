﻿using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.Admin
{
    public class TutorDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public ItemState State { get; set; }
    }
}