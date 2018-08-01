namespace Cloudents.Ico.Models
{
    public class EidanTeam : Team
    {
        public EidanTeam() : base("Eidan")
        {
        }

        public override string Image => "/images/muscari/Eidan.png";
        public override string LinkdinLink => "https://www.linkedin.com/in/apelbaum";
        protected override int Order => 1;
    }
}