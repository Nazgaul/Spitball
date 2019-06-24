﻿using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class ConversationParamsResponse
    {
        public IEnumerable<string> Status { get; set; }
        public IEnumerable<string> AssignTo { get; set; }
        public IEnumerable<string> WaitingFor { get; set; }
      

    }
}
