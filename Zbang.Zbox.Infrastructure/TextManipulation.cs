using System.Text.RegularExpressions;

namespace Zbang.Zbox.Infrastructure
{
    public static class TextManipulation
    {
        public static readonly Regex RemoveHtmlTags = new Regex("<.*?>|&nbsp;|&zwnj;|&raquo;|&laquo;", RegexOptions.Compiled);
        public static readonly Regex SpaceReg = new Regex(@"\s+", RegexOptions.Compiled);
        public static readonly Regex RemoveUnwantedUrlChars = new Regex(@"[""!~<>^*&`#'()%$:/;?@=+,{}|^[\]\\\u00A0]", RegexOptions.Compiled);
        public static readonly Regex UrlSpace = new Regex(@"\s+|[-_]", RegexOptions.Compiled);
        //public static readonly Regex UrlDetector = new Regex(@"(?i)\b((?:https?://|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
