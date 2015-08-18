using System.Text.RegularExpressions;

namespace Zbang.Zbox.Infrastructure
{
    public static class TextManipulation
    {
        public static readonly Regex RemoveHtmlTags = new Regex("<.*?>", RegexOptions.Compiled);
    }
}
