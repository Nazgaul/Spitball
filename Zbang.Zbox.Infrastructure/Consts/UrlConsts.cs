﻿using System;
using System.Text;
using System.Web;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Zbox.Infrastructure.Consts
{
    public static class UrlConst
    {
        public const string CloudentsUrl = "https://www.spitball.co";
        private const string ItemUrl = "/item/{0}/{1}/{2}/{3}/{4}/";
        private const string QuizUrl = "/quiz/{0}/{1}/{2}/{3}/{4}/";
        private const string BoxUrl = "/box/my/{0}/{1}/";
        private const string CourseUrl = "/course/{2}/{0}/{1}/";
        private const string UserUrl = "/user/{0}/{1}/";
        private const string LogInUrl = "/?invId={0}";

        public const string ShortBox = "b/{box62Id}";
        public const string ShortItem = "i/{item62Id}";

        public static string BuildShortBoxUrl(string box62Id)
        {
            return "/" + ShortBox.Replace("{box62Id}", box62Id);
        }

        public static string BuildShortItemUrl(string item62Id)
        {
            return "/" + ShortItem.Replace("{item62Id}", item62Id);
        }

        public const string PasswordUpdate = CloudentsUrl + "/account/passwordupdate?key={0}";
        private const string BoxUrlInvite = "?invId={0}";

        public static string BuildInviteUrl(string boxUrl, string inviteId)
        {
            return CloudentsUrl + boxUrl + string.Format(BoxUrlInvite, inviteId);
        }

        public static string AppendCloudentsUrl(string relativeUrl)
        {
            if (relativeUrl.StartsWith(Uri.UriSchemeHttps, StringComparison.InvariantCultureIgnoreCase))
            {
                return relativeUrl;
            }
            return HttpUtility.UrlPathEncode(CloudentsUrl + relativeUrl);
            
        }


        public static string BuildInviteCloudentsUrl(string inviteId)
        {
            return CloudentsUrl + string.Format(LogInUrl, inviteId);
        }

        public static string BuildUserUrl(long id, string name, bool fullUrl = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name");
            }

            var relativeUrl = VirtualPathUtility.AppendTrailingSlash(string.Format(UserUrl, id, NameToQueryString(name)));
            if (fullUrl)
            {
                return CloudentsUrl + relativeUrl;
            }
            return relativeUrl;
        }

        public static string BuildBoxUrl(long id, string name, string universityName, bool fullUrl = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }
            string relativeUrl = VirtualPathUtility.AppendTrailingSlash(string.IsNullOrEmpty(universityName) ? string.Format(BoxUrl, id, NameToQueryString(name)) : string.Format(CourseUrl, id, NameToQueryString(name), NameToQueryString(universityName)));
            if (fullUrl)
            {
                return CloudentsUrl + relativeUrl;
            }
            return relativeUrl;
        }





        public static string BuildItemUrl(long boxId, string boxName, long itemId, string itemName, string universityName, bool fullUrl = false)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                throw new ArgumentException("itemName");
            }
            if (string.IsNullOrEmpty(boxName))
            {
                throw new ArgumentException("boxName");
            }


            var relativeUrl = VirtualPathUtility.AppendTrailingSlash(string.Format(ItemUrl,
                NameToQueryString(universityName), boxId, NameToQueryString(boxName), itemId, NameToQueryString(itemName)));
            if (fullUrl)
            {
                return CloudentsUrl + relativeUrl;
            }
            return relativeUrl;
        }
        public static string BuildQuizUrl(long boxId, string boxName, long quizId, string quizName, string universityName, bool fullUrl = false)
        {
            if (string.IsNullOrEmpty(quizName))
            {
                throw new ArgumentException(nameof(quizName));
            }
            if (string.IsNullOrEmpty(boxName))
            {
                throw new ArgumentException(nameof(boxName));
            }


            var relativeUrl = VirtualPathUtility.AppendTrailingSlash(string.Format(QuizUrl, NameToQueryString(universityName), boxId, NameToQueryString(boxName), quizId, NameToQueryString(quizName)));
            if (fullUrl)
            {
                return CloudentsUrl + relativeUrl;
            }
            return relativeUrl;
        }



       

        //public static string BuildLibraryUrl(Guid id, string name)
        //{
        //    if (string.IsNullOrEmpty(name))
        //    {
        //        throw new ArgumentNullException(nameof(name));
        //    }
        //    return VirtualPathUtility.AppendTrailingSlash(
        //        string.Format(
        //            LibraryUrl, 
        //            GuidEncoder.Encode(id), 
        //            NameToQueryString(name)));
        //}

        public static string NameToQueryString(string name)
        {
            //var outString =  sourceString.replace(/[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi, '');
            // - < > " ' % ; ) ( & + - 

            //<,>,*,%,&,:,\\
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }
            char previousChar = '\0';
            var sb = new StringBuilder();
           // name = name.Replace(Convert.ToChar(160), ' ');
            //name = name.Replace("\n", string.Empty);

            foreach (var character in name)
            {

                if (!char.IsLetterOrDigit(character) && !char.IsWhiteSpace(character) && !char.IsPunctuation(character))
                    continue;
                switch (character)
                {
                    case (char)160:
                    case '\n':
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
                    case '#':
                    case '\'':
                    case '(':
                    case ')':
                        continue;
                    case ' ':
                    case '_':
                    case '-':
                    case (char)65288:
                    case (char)65289:
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

    public static class MetadataConst
    {
        public const string VideoStatus = "VideoStatus";
    }
}
