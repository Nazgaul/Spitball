﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class AddCommentRequest
    {
        [Required]
        public string Content { get; set; }
    }
}