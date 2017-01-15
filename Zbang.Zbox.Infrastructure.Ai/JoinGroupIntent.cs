namespace Zbang.Zbox.Infrastructure.Ai
{
    public class JoinGroupIntent : IIntent
    {
       // public string SearchQuery { get; set; }
        public string UniversityName { get; set; }
        public string CourseName { get; set; }
        public long Time { get; set; }


        public override string ToString()
        {
            var str = "you want to join to study group";
            if (string.IsNullOrEmpty(UniversityName))
            {
                str += " i didn't got your university";
            }
            else
            {
                str += " in university " + UniversityName;
            }
            if (string.IsNullOrEmpty(CourseName))
            {
                str += " i didn't got your course you need help";
            }
            else
            {
                str += "  in course " + CourseName;
            }
            return str;
        }
    }
}