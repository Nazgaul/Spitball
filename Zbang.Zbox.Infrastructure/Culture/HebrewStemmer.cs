
using System;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Culture
{
    public class HebrewStemmer : IHebrewStemmer
    {
        public string StemAHebrewWord(string phrase)
        {
            if (phrase == null) throw new ArgumentNullException("phrase");
            var sb = new StringBuilder();

            var words = phrase.Split(' ');

            
            foreach (var word in words)
            {
                sb.Append(RemoveStatingHeigh(word) + "%");
                
            }
            
            return sb.ToString().Trim();
        }

        private string RemoveStatingHeigh(string word)
        {
            if (word.StartsWith("ה"))
            {
                return word.Substring(1);
            }
            return word;
        }


    }
}
