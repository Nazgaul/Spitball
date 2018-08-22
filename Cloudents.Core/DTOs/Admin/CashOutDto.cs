﻿using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class CashOutDto
    {
        public long UserId { get; set; }
        public decimal CashOutPrice { get; set; }
        public DateTime CashOutTime { get; set; }
        public bool IsSuspect { get; set; }
    }
}
