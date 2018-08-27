namespace Cloudents.Ico.Models
{
    public class HadarTeam : Team
    {
        public HadarTeam() : base("Hadar")
        {
        }

        public override string Image => "/images/muscari/Hadar.png";
        public override string LinkdinLink => "https://www.linkedin.com/in/hadar-keidar-167786124/";
        protected override int Order => 6;
    }
 }

