﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class AI
    {
        [Required(AllowEmptyStrings = false)]
        public string Sentence { get; set; }
    }
}
