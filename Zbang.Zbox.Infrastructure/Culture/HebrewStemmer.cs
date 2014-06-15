
using System.Text;

namespace Zbang.Zbox.Infrastructure.Culture
{
    public class HebrewStemmer : IHebrewStemmer
    {
        public string StemAHebrewWord(string phrase)
        {
            var sb = new StringBuilder();

            var words = phrase.Split(' ');

            
            foreach (var word in words)
            {
                sb.Append(removeStatingHeigh(word) + "%");
                
            }
            
            return sb.ToString().Trim();
        }

        private string removeStatingHeigh(string word)
        {
            if (word.StartsWith("ה"))
            {
                return word.Substring(1);
            }
            return word;
        }


    }
}
