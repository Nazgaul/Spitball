namespace Zbang.Zbox.Infrastructure.Ai
{
    public class HomeWorkIntent : IIntent
    {
        public string UniversityName { get; set; }
        public string CourseName { get; set; }

        public string SearchQuery { get; set; }
        //public long Time { get; set; }


        public override string ToString()
        {
            var str = "you need help in home work.";
            if (string.IsNullOrEmpty(UniversityName))
            {
                str += " i didn't got your university";
            }
            else
            {
                str += " you study in university " + UniversityName;
            }
            if (string.IsNullOrEmpty(CourseName))
            {
                str += " i didn't got your course you need help";
            }
            else
            {
                str += " you study in course " + CourseName;
            }
            if (string.IsNullOrEmpty(SearchQuery))
            {
                str += " but i dont understand what is the problem";
            }
            else
            {
                str += " and your problem is " + SearchQuery;
            }
            return str;
        }
    }
}