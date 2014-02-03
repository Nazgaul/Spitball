using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Culture
{
    public class EnglishToHebrewChars : IEnglishToHebrewChars
    {
        private readonly Dictionary<char, char> m_EnglishToHebrewKeyboard = new Dictionary<char, char>
        {
            {'q','/'},
            {'w','\''},
            {'e','ק'},
            {'r','ר'},
            {'t','א'},
            {'y','ט'},
            {'u','ו'},
            {'i','ן'},
            {'o','ם'},
            {'p','פ'},
            {'a','ש'},
            {'s','ד'},
            {'d','ג'},
            {'f','כ'},
            {'g','ע'},
            {'h','י'},
            {'j','ח'},
            {'k','ל'},
            {'l','ך'},
            {';','ף'},
            {'z','ז'},
            {'x','ס'},
            {'c','ב'},
            {'v','ה'},
            {'b','נ'},
            {'n','מ'},
            {'m','צ'},
            {',','ת'},
            {'.','ץ'}
        };


        public string TransferEnglishCharsToHebrew(string englishSentece)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var letter in englishSentece.ToLower())
            {
                var outLetter = letter;
                if (m_EnglishToHebrewKeyboard.ContainsKey(letter))
                {
                    outLetter = m_EnglishToHebrewKeyboard[letter];
                }
                sb.Append(outLetter);
            }
            return sb.ToString();
        }
    }
}
