using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Cloudents.Ico.Resources.Model;

namespace Cloudents.Ico.Models
{

    public class RamTeam : Team
    {
        public RamTeam() : base("Ram")
        {
        }

        public override string Image => "/images/muscari/Ram.png";
        public override string LinkdinLink => "https://www.linkedin.com/in/ramyaari/";
    }
}
