using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public class QuestionIntent : KnownIntent
    {
        public override string ToString()
        {
            var listOfStr = new List<string> { "You wanted to ask a question" };

            
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

        public override SearchType SearchType => SearchType.Feed;
    }

}