﻿using System;
using System.Text;
using System.Web;

namespace Zbang.Zbox.Infrastructure.Consts
{
    public class UrlConsts
    {
        public const string CloudentsUrl = "https://www.cloudents.com/";
        public const string ItemUrl = CloudentsUrl + "item/{0}/{1}/{2}/{3}/{4}/";
        public const string QuizUrl = CloudentsUrl + "quiz/{0}/{1}/{2}/{3}/{4}/";
        public const string BoxUrl = CloudentsUrl + "box/my/{0}/{1}/";
        public const string CourseUrl = CloudentsUrl + "course/{2}/{0}/{1}/";
        private const string UserUrl = "/user/{0}/{1}";
        public const string PasswordUpdate = CloudentsUrl + "account/passwordupdate?key={0}";
        public const string BoxUrlInvite = CloudentsUrl + "Share/FromEmail?key={0}&email={1}";

        public static string BuildUserUrl(long id, string name, bool fullUrl = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name");
            }
            
            var relativeUrl = VirtualPathUtility.AppendTrailingSlash(string.Format(UserUrl, id, NameToQueryString(name)));
            if (fullUrl)
            {
                return VirtualPathUtility.AppendTrailingSlash(CloudentsUrl) + relativeUrl;
            }
            return relativeUrl;
        }
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
