namespace Zbang.Zbox.Infrastructure.Ai
{
    public class ResearchIntent : IIntent
    {
        public string SearchQuery { get; set; }
        public override string ToString()
        {
            var str = "you need help in research.";
           
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