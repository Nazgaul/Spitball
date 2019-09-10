using System.Text.RegularExpressions;

namespace Cloudents.Core
{
    public static class RegEx
    {
        public static readonly Regex RemoveHtmlTags = new Regex("<.*?>|&nbsp;|&zwnj;|&raquo;|&laquo;", RegexOptions.Compiled);

        public static readonly Regex RtlLetters =
            new Regex("[\x0590-\x05ff\x0600-\x06ff]", RegexOptions.Compiled);

        // public static readonly Regex SpaceReg = new Regex(@"\s+", RegexOptions.Compiled);
    }
}
