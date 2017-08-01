using System.Linq;
using Abot.Core;
using Abot.Poco;
using AbotX.Core;
using AbotX.Poco;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class CrawlDecisionSpitball : CrawlDecisionMakerX
    {

        public CrawlDecisionSpitball(CrawlConfigurationX config) : base(config)
        {

        }

        public override CrawlDecision ShouldCrawlPage(PageToCrawl pageToCrawl, CrawlContext crawlContext)
        {
            if (pageToCrawl.Uri.PathAndQuery.ToLowerInvariant().Contains("xml"))
            {
                return new CrawlDecision
                {
                    Allow = true
                };
            }
            if (pageToCrawl.Uri.Authority.Equals("spitball.co", System.StringComparison.InvariantCultureIgnoreCase))
            {
                if (pageToCrawl.Uri.PathAndQuery.Contains("returnUrl"))
                {
                    return new CrawlDecision { Allow = false, Reason = "This is redirect Url" };
                }
            }

            if (pageToCrawl.Uri.Authority.Equals("studysoup.com", System.StringComparison.InvariantCultureIgnoreCase))
            {

                var segments = pageToCrawl.Uri.Segments;

                if (segments.Length < 2)
                {
                    return new CrawlDecision { Allow = false, Reason = "Not Part of studysoup white list" };
                }
                var absPath = pageToCrawl.Uri.Segments[1].ToLowerInvariant().Replace("/", string.Empty);
                var whiteList = new[] { "flashcard", "note", "guide", "bundle" };

                if (whiteList.Contains(absPath))
                {
                    return base.ShouldCrawlPage(pageToCrawl, crawlContext);

                }
                return new CrawlDecision { Allow = false, Reason = "Not Part of studysoup white list" };
            }
            else if (pageToCrawl.Uri.Authority.Equals("www.khanacademy.org"))
            {
                var segments = pageToCrawl.Uri.Segments;

                if (segments.Length < 4)
                {
                    return new CrawlDecision { Allow = false, Reason = "Not Part of Khanan academy white list" };
                }
                var absPath = segments[1].ToLowerInvariant().Replace("/", string.Empty);
                var secondSeg = segments[2].ToLowerInvariant();
                //Ignore math of high-school class
                var mathSchool = secondSeg.StartsWith("cc-") || secondSeg.Contains("-grade") || secondSeg.Contains("engageny") || secondSeg.Contains("-engage-");
                var whiteList = new[] { "math", "science", "computing", "humanities", "economics-finance-domain" };
                //Validate whitelist and math sub units is not regarding school
                if (whiteList.Contains(absPath) && (!absPath.Equals("math") || !mathSchool))
                {
                    return base.ShouldCrawlPage(pageToCrawl, crawlContext);
                }
                else
                {
                    return new CrawlDecision { Allow = false, Reason = "Not Part of Khanan white list" };
                }
            }
            return base.ShouldCrawlPage(pageToCrawl, crawlContext);
        }


        //public CrawlDecision ShouldCrawlSite(SiteToCrawl siteToCrawl)
        //{
        //    return new CrawlDecision { Allow = true };
        //}

        public override CrawlDecision ShouldRenderJavascript(CrawledPage crawledPage, CrawlContext crawlContext)
        {
            var contentType = crawledPage.HttpWebResponse?.ContentType?.ToLowerInvariant();
            if (contentType?.Contains("xml") == true)
            {
                return new CrawlDecision { Allow = false, Reason = "not an html page" };
            }
            if (contentType?.Contains("application/octet-stream") == true)
            {
                return new CrawlDecision { Allow = false, Reason = "not an html page" };
            }
            return base.ShouldRenderJavascript(crawledPage, crawlContext);
            //if (crawledPage.HttpWebResponse?.ContentType?.ToLowerInvariant().Contains("text/html") == true)
            //{

            //    return new CrawlDecision { Allow = true };
            //}

            //return new CrawlDecision { Allow = false, Reason = "not an html page" };
        }
    }
}
