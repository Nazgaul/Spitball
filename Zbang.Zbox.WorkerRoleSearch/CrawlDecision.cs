using System.Linq;
using Abot.Core;
using Abot.Poco;
using AbotX.Core;
using AbotX.Poco;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class CrawlDecisionSpitball : CrawlDecisionMakerX
    {

        public CrawlDecisionSpitball(CrawlConfigurationX config) :base (config)
        {
            
        }

        public override CrawlDecision ShouldCrawlPage(PageToCrawl pageToCrawl, CrawlContext crawlContext)
        {

            if (pageToCrawl.Uri.Authority.Equals("spitball.co",System.StringComparison.InvariantCultureIgnoreCase))
            {
                if (pageToCrawl.Uri.PathAndQuery.Contains("returnUrl"))
                {
                    return new CrawlDecision { Allow = false, Reason = "This is redirect Url" };
                }
            }

            if (pageToCrawl.Uri.Authority.Equals("studysoup.com", System.StringComparison.InvariantCultureIgnoreCase))
            {
                if (pageToCrawl.Uri.PathAndQuery.ToLowerInvariant().Contains("xml"))
                {
                    return new CrawlDecision
                    {
                        Allow = true
                    };
                }
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
