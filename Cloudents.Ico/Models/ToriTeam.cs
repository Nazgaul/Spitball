using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Ico.Models
{
    public class ToriTeam : Team
    {
        public ToriTeam() : base("Tori")
        {
        }

        public override string Image => "/images/muscari/Tori.png";
        public override string LinkdinLink => "https://www.linkedin.com/in/toriwithee/";
    }
    
}
