using System;
using System.Text;
using System.Web;

namespace Zbang.Zbox.Infrastructure.Consts
{
    public static class UrlConsts
    {
        private const string CloudentsUrl = "https://www.cloudents.com";
        private const string ItemUrl = "/item/{0}/{1}/{2}/{3}/{4}/";
        private const string QuizUrl = "/quiz/{0}/{1}/{2}/{3}/{4}/";
        private const string BoxUrl = "/box/my/{0}/{1}/";
        private const string CourseUrl = "/course/{2}/{0}/{1}/";
        private const string UserUrl = "/user/{0}/{1}";

        private const string StoreCategoryUrl = "/store/category/{0}/{1}/";
        private const string StoreProductUrl = "/store/product/{0}/{1}/";



        public const string PasswordUpdate = CloudentsUrl + "/account/passwordupdate?key={0}";
        public const string BoxUrlInvite = CloudentsUrl + "/Share/FromEmail?key={0}&email={1}";

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
            string relativeUrl;
            if (string.IsNullOrEmpty(universityName))
            {
                relativeUrl = VirtualPathUtility.AppendTrailingSlash(string.Format(BoxUrl, id, NameToQueryString(name)));
            }
            else
            {
                relativeUrl = VirtualPathUtility.AppendTrailingSlash(string.Format(CourseUrl, id, NameToQueryString(name), NameToQueryString(universityName)));
            }
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


            var relativeUrl = VirtualPathUtility.AppendTrailingSlash(string.Format(ItemUrl, NameToQueryString(universityName), boxId, NameToQueryString(boxName), itemId, NameToQueryString(itemName)));
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
                throw new ArgumentException("itemName");
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


        public static string BuildStoreCatergoryUrl(int categoryId, string categoryName)
        {
            if (categoryName == null) throw new ArgumentNullException("categoryName");
            return VirtualPathUtility.AppendTrailingSlash(string.Format(StoreCategoryUrl, categoryId, NameToQueryString(categoryName)));
        }
        public static string BuildStoreProductUrl(long productId, string productName)
        {
            if (productName == null) throw new ArgumentNullException("productName");
            return VirtualPathUtility.AppendTrailingSlash(string.Format(StoreProductUrl, productId, NameToQueryString(productName)));
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
