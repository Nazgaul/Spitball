using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Ico.Models
{
    public class JordanTeam : Team
    {
        public JordanTeam() : base("Jordan")
        {
        }

        public override string Image => "/images/muscari/Jordan.png";
        public override string LinkdinLink => "https://www.linkedin.com/in/jordanweiss/";
    }
}
