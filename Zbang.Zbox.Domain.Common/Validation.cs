using System.Text.RegularExpressions;

namespace Zbang.Zbox.Domain.Common
{
    public static class Validation
    {


        #region Url

        private const string UrlScheme = "^https?://";

        public static bool IsUrlWithoutScheme(string url)
        {
            return !Regex.IsMatch(url, UrlScheme, RegexOptions.IgnoreCase);

        }
        #endregion

       
    }


}
