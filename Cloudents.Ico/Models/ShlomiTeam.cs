namespace Cloudents.Ico.Models
{
    public class ShlomiTeam : Team
    {
        public ShlomiTeam() : base("Shlomi")
        {
        }

        public override string Image => "/images/muscari/Shlomi.png";
        public override string LinkdinLink => "https://www.linkedin.com/in/kastoryano/";
        protected override int Order => 5;
    }
}
