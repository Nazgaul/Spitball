﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class MarkAnswer
    {
        [Required]
        public Guid? AnswerId { get; set; }


    }
}