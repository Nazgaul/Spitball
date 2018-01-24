using System.Text.RegularExpressions;

namespace Cloudents.Core
{
    public static class RegEx
    {
        public static readonly Regex RemoveHtmlTags = new Regex("<.*?>|&nbsp;|&zwnj;|&raquo;|&laquo;", RegexOptions.Compiled);
        public static readonly Regex SpaceReg = new Regex(@"\s+", RegexOptions.Compiled);
    }
}
