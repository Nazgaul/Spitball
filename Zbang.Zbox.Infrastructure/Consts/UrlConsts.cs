using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Consts
{
    public class UrlConsts
    {
        public const string CloudentsURL = "https://www.cloudents.com/";
        public const string ItemUrl = CloudentsURL + "item/{0}/{1}/{2}/{3}/{4}/";
        public const string QuizUrl = CloudentsURL + "quiz/{0}/{1}/{2}/{3}/{4}/";
        public const string BoxUrl = CloudentsURL + "box/my/{0}/{1}/";
        public const string CourseUrl = CloudentsURL + "course/{2}/{0}/{1}/";
        public const string UserUrl = CloudentsURL + "user/{0}/{1}";
        public const string PasswordUpdate = "https://www.cloudents.com/account/passwordupdate?key={0}";
        public const string BoxUrlInvite = "https://www.cloudents.com/Share/FromEmail?key={0}&email={1}";

        public static string NameToQueryString(string name)
        {
            // - < > " ' % ; ) ( & + - 

            //<,>,*,%,&,:,\\
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }
            char previousChar = '\0';
            var sb = new StringBuilder();
            foreach (var character in name)
            {
                switch (character)
                {
                    case '<':
                    case '>':
                    case '*':
                    case '%':
                    case '&':
                    case ':':
                    case '\\':
                    case '/':
                    case ';':
                    case '?':
                    case '@':
                    case '=':
                    case '+':
                    case '$':
                    case ',':
                    case '{':
                    case '}':
                    case '|':
                    case '^':
                    case '[':
                    case ']':
                    case '`':
                    case '"':
                    case '\'':
                        continue;
                    case ' ':
                    case '_':
                    case '-':
                        if (previousChar != '-')
                        {
                            sb.Append('-');
                        }
                        previousChar = '-';

                        break;
                    default:
                        previousChar = character;
                        sb.Append(character);
                        break;
                }
            }
            return sb.ToString().ToLower();

        }
    }

    public class MetaDataConsts
    {
        public const string VideoStatus = "VideoStatus";
    }
}
