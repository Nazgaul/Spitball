using System;
using System.Linq;
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
                return new CrawlDecision
                {
                    Allow = true
                };
            if (pageToCrawl.Uri.Authority.Equals("spitball.co", StringComparison.InvariantCultureIgnoreCase))
                if (pageToCrawl.Uri.PathAndQuery.Contains("returnUrl"))
                    return new CrawlDecision {Allow = false, Reason = "This is redirect Url"};

            if (pageToCrawl.Uri.Authority.Equals("studysoup.com", StringComparison.InvariantCultureIgnoreCase))
            {
                var segments = pageToCrawl.Uri.Segments;

                if (segments.Length < 2)
                    return new CrawlDecision {Allow = false, Reason = "Not Part of studysoup white list"};
                var absPath = pageToCrawl.Uri.Segments[1].ToLowerInvariant().Replace("/", string.Empty);
                var whiteList = new[] {"flashcard", "note", "guide", "bundle"};

                if (whiteList.Contains(absPath))
                    return base.ShouldCrawlPage(pageToCrawl, crawlContext);
                return new CrawlDecision {Allow = false, Reason = "Not Part of studysoup white list"};
            }
            else if (pageToCrawl.Uri.Authority.Equals("quizlet.com"))
            {
                var segments = pageToCrawl.Uri.Segments;
                var blackList = new[] { "flashcards", "learn", "spell", "match", "test", "gravity" };
                if (segments.Length >= 3 && blackList.Contains(segments[2].ToLowerInvariant().Replace("/", string.Empty)))
                    return new CrawlDecision { Allow = false, Reason = "Not neef in mapping" };
                //TODO: uncomment when using sitemap
                //var absPath = pageToCrawl.Uri.Segments[1].ToLowerInvariant().Replace("/", string.Empty);
                //if (int.TryParse(absPath,out int quizNum))
                //    return base.ShouldCrawlPage(pageToCrawl, crawlContext);
                //return new CrawlDecision { Allow = false, Reason = "Not Part of Quizlet white list" };
            }
            else if (pageToCrawl.Uri.Authority.Equals("www.khanacademy.org"))
            {
                var segments = pageToCrawl.Uri.Segments;

                if (segments.Length < 4)
                    return new CrawlDecision {Allow = false, Reason = "Not Part of Khanan academy white list"};
                var absPath = segments[1].ToLowerInvariant().Replace("/", string.Empty);
                var secondSeg = segments[2].ToLowerInvariant();
                //Ignore math of high-school class
                var mathSchool = new[] {"early-math", "arithmetic"}.Contains(secondSeg) ||
                                 secondSeg.StartsWith("cc-") ||
                                 secondSeg.Contains("-grade") || secondSeg.Contains("engageny") ||
                                 secondSeg.Contains("-engage-");
                var whiteList = new[] {"math", "science", "computing", "humanities", "economics-finance-domain"};
                //Validate whitelist and math sub units is not regarding school
                if (whiteList.Contains(absPath) && (!absPath.Equals("math") || !mathSchool))
                    return base.ShouldCrawlPage(pageToCrawl, crawlContext);
                return new CrawlDecision {Allow = false, Reason = "Not Part of Khanan white list"};
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
                return new CrawlDecision {Allow = false, Reason = "not an html page"};
            if (contentType?.Contains("application/octet-stream") == true)
                return new CrawlDecision {Allow = false, Reason = "not an html page"};
            return base.ShouldRenderJavascript(crawledPage, crawlContext);
            //if (crawledPage.HttpWebResponse?.ContentType?.ToLowerInvariant().Contains("text/html") == true)
            //{

            //    return new CrawlDecision { Allow = true };
            //}

            //return new CrawlDecision { Allow = false, Reason = "not an html page" };
        }
    }
}
