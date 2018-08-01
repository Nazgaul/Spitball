using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Ico.Models
{
    public class RobertTeam : Team
    {
        public RobertTeam() : base("Robert")
        {
        }

        public override string Image => "/images/muscari/Rob.png";
        public override string LinkdinLink => "https://www.linkedin.com/in/robertecohen/";
    }
}
