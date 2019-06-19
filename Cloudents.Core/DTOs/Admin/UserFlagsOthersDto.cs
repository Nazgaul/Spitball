﻿using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserFlagsOthersDto
    {
        [EntityBind(nameof(BaseUser.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(BaseUser.Country))]
        public string Country { get; set; }
        public int Flags { get; set; }
        public static int Rows { get; set; }
    }
}
