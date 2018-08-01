using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Ico.Models
{
    public class ShlomiTeam : Team
    {
        public ShlomiTeam() : base("Shlomi")
        {
        }

        public override string Image => "/images/muscari/Shlomi.png";
        public override string LinkdinLink => "https://www.linkedin.com/in/kastoryano/";
    }
}
