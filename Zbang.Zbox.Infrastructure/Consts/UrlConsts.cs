using System;
using System.Text;
using System.Web;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Zbox.Infrastructure.Consts
{
    public static class UrlConsts
    {
        public const string CloudentsUrl = "https://www.cloudents.com";
        private const string ItemUrl = "/item/{0}/{1}/{2}/{3}/{4}/";
        private const string QuizUrl = "/quiz/{0}/{1}/{2}/{3}/{4}/";
        private const string BoxUrl = "/box/my/{0}/{1}/";
        private const string CourseUrl = "/course/{2}/{0}/{1}/";
        private const string UserUrl = "/user/{0}/{1}/";
        private const string LibraryUrl = "/library/{0}/{1}/";
        private const string StoreProductUrl = "/store/product/{0}/{1}/";
        private const string LogInUrl = "/account/?invId={0}";


        public const string PasswordUpdate = CloudentsUrl + "/account/passwordupdate?key={0}";
        private const string BoxUrlInvite = "?invId={0}";

        public static string BuildInviteUrl(string boxUrl, string invId)
        {
            return CloudentsUrl + boxUrl + string.Format(BoxUrlInvite, invId);
        }

        public static string AppendCloudentsUrl(string relativeUrl)
        {
            return HttpUtility.UrlPathEncode(CloudentsUrl + relativeUrl);
        }

        public static string BuildInviteCloudentsUrl(string invId)
        {
            return CloudentsUrl + string.Format(LogInUrl, invId);
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
                throw new ArgumentException("name");
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
                throw new ArgumentException("quizName");
            }
            if (string.IsNullOrEmpty(boxName))
            {
                throw new ArgumentException("boxName");
            }


            var relativeUrl = VirtualPathUtility.AppendTrailingSlash(string.Format(QuizUrl, NameToQueryString(universityName), boxId, NameToQueryString(boxName), quizId, NameToQueryString(quizName)));
            if (fullUrl)
            {
                return CloudentsUrl + relativeUrl;
            }
            return relativeUrl;
        }



        public static string BuildStoreProductUrl(long productId, string productName)
        {
            if (productName == null) throw new ArgumentNullException("productName");
            return VirtualPathUtility.AppendTrailingSlash(string.Format(StoreProductUrl, productId, NameToQueryString(productName)));
        }

        public static string BuildLibraryUrl(Guid id, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name");
            }
            return VirtualPathUtility.AppendTrailingSlash(string.Format(LibraryUrl, GuidEncoder.Encode(id), NameToQueryString(name)));
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
            name = name.Replace(Convert.ToChar(160), ' ');

            foreach (var character in name)
            {
                if (!char.IsLetterOrDigit(character) && !char.IsWhiteSpace(character) && !char.IsPunctuation(character))
                    continue;
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
                    case '#':
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

    public static class MetaDataConsts
    {
        public const string VideoStatus = "VideoStatus";
    }
}
