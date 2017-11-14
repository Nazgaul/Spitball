namespace Cloudents.Web.Models
{
    public class SiteMapLangNode
    {
        public SiteMapLangNode(string url, string language)
        {
            Url = url;
            Language = language;
        }

        public string Url { get; }
        public string Language { get; }
    }
}
