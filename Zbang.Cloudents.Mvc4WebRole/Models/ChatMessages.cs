﻿using System;
using System.Collections.Generic;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class ChatMessages
    {
       
        public Guid? ChatRoom { get; set; }
        public IEnumerable<long> UserIds { get; set; }

        public DateTime? DateTime { get; set; }

        public int Top { get; set; }
    }
}