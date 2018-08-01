namespace Cloudents.Ico.Models
{

    public class RamTeam : Team
    {
        public RamTeam() : base("Ram")
        {
        }

        public override string Image => "/images/muscari/Ram.png";
        public override string LinkdinLink => "https://www.linkedin.com/in/ramyaari/";
        protected override int Order => 2;
    }
}
