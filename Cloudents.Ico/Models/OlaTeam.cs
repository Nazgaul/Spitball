﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Ico.Models
{
    public class OlaTeam : Team
    {
        public OlaTeam() : base("Ola")
        {
        }

        public override string Image => "/images/muscari/Ola.png";
        public override string LinkdinLink => "https://www.linkedin.com/in/aleksandra-jankowska-28a3bb126/?locale=en_US";
    }
    
}
