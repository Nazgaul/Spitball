using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public class SearchIntent : BaseIntent
    {
        [WitApiName("SearchType")]
        public string SearchType { get; set; }

        public override string ToString()
        {
            var listOfStr = new List<string> {"You wanted to search"};


            if (string.IsNullOrEmpty(SearchType))
            {
                listOfStr.Add("I don't know the search type");
            }
            else
            {
                listOfStr.Add("The type of search is" + SearchType);
            }
            if (string.IsNullOrEmpty(University))
            {
                listOfStr.Add("I don't know the university");
            }
            else
            {
                listOfStr.Add("In university " + University);
            }
            if (string.IsNullOrEmpty(Course))
            {
                listOfStr.Add("I don't know which class");
            }
            else
            {
                listOfStr.Add("In Class " + Course);
            }
            if (string.IsNullOrEmpty(Term))
            {
                listOfStr.Add("I don't know the term");
            }
            else
            {
                listOfStr.Add("The term is " + Term);
            }
            return string.Join(". ", listOfStr);
        }
    }
}
