using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserFlagsOthersDto
    {
        [DtoToEntityConnection(nameof(User.Id))]
        public long Id { get; set; }
        [DtoToEntityConnection(nameof(User.Country))]
        public string Country { get; set; }
        public int Flags { get; set; }
        public static int Rows { get; set; }
    }
}
