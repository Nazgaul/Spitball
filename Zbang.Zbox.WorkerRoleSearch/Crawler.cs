using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Abot.Poco;
using AbotX.Core;
using AbotX.Crawler;
using AbotX.Poco;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class Crawler : ISchedulerProcess, IJob, IDisposable
    {
        private readonly CrawlerX m_Crawler;
        private readonly IBlobProvider2<CrawlContainerName> m_BlobProvider;
        private readonly IDocumentDbRepository<CrawlModel> m_DocumentDbRepository;
        private readonly IMailComponent m_MailManager;
        TelemetryClient telemetry = new TelemetryClient();

        public Crawler(IBlobProvider2<CrawlContainerName> blobProvider, IDocumentDbRepository<CrawlModel> documentDbRepository, IMailComponent mailManager)
        {
            m_BlobProvider = blobProvider;
            m_DocumentDbRepository = documentDbRepository;
            m_MailManager = mailManager;
            var finder = new CrawlSiteMapFinder();
            var config = AbotXConfigurationSectionHandler.LoadFromXml().Convert();


            var implementation = new ImplementationOverride(config, new ImplementationContainer
            {
                HyperlinkParser = finder,
                Scheduler = new CrawlScheduler2(config.IsUriRecrawlingEnabled),
                CrawlDecisionMakerX = new CrawlDecisionSpitball(config),
                CrawlDecisionMaker = new CrawlDecisionSpitball(config)
            });
            m_Crawler = new CrawlerX(config, implementation);
            m_Crawler.PageCrawlCompletedAsync += CrawlCompleted;
            //m_Crawler.PageCrawlDisallowed += Crawler_PageCrawlDisallowed;
        }

        private void CrawlCompleted(object sender, Abot.Crawler.PageCrawlCompletedArgs e)
        {
            var crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                TraceLog.WriteWarning($"Crawl of page failed {crawledPage.Uri.AbsoluteUri} , exception {crawledPage.WebException}");
                return;
            }

            if (!crawledPage.HttpWebResponse.ContentType.Contains("text/html"))
            {
                TraceLog.WriteWarning($"Crawl of page {crawledPage.Uri.AbsoluteUri} of type {crawledPage.HttpWebResponse.ContentType}");
                return;

            }

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
            {
                TraceLog.WriteWarning($"Page had no content {crawledPage.Uri.AbsoluteUri}");
                return;
            }
            if (crawledPage.Uri.Authority.Equals("studysoup.com"))
            {
                try
                {
                    
                    var model = CreateStudySoupNote(crawledPage);
                    var str = JsonConvert.SerializeObject(model);
                    m_BlobProvider.UploadText(model.Id, str);
                    m_DocumentDbRepository.CreateItemAsync(model).Wait();
                }
                catch (Exception ex)
                {
                    telemetry.TrackException(ex, new Dictionary<string, string> {{"page", crawledPage.ToString()}});
                    TraceLog.WriteError($"on parsing study soup web {crawledPage}", ex);
                }
            }
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var studySoupSiteMap = new Uri("https://studysoup.com/sitemap.xml.gz");
            //var tempUrl =
            //    new Uri("https://studysoup.com/note/17394/ui-econ-1100-0aaa-week-10-spring-2015-kelsy-lartius");
            var t = m_Crawler.CrawlAsync(studySoupSiteMap);

            while (!cancellationToken.IsCancellationRequested && !t.IsCompleted)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {

                }
                finally
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        m_Crawler.Stop();
                    }
                }
            }
            await t.ConfigureAwait(false);
            var result = t.Result;
            if (result.ErrorOccurred)
            {
                await m_MailManager.GenerateSystemEmailAsync("crawler",
                    $"Crawl of {result.RootUri.AbsoluteUri} completed with error: {result.ErrorException.Message}").ConfigureAwait(false);
                TraceLog.WriteError(
                    $"Crawl of {result.RootUri.AbsoluteUri} completed with error: {result.ErrorException.Message}");
            }
            else
            {
                await m_MailManager.GenerateSystemEmailAsync("crawler",
                    $"Crawl of {result.RootUri.AbsoluteUri} completed without error.").ConfigureAwait(false);
                TraceLog.WriteInfo($"Crawl of {result.RootUri.AbsoluteUri} completed without error.");

            }

        }

        public string Name => nameof(Crawler);


        private static CrawlModel CreateStudySoupNote(CrawledPage page)
        {
            var angleSharpHtmlDocument = page.AngleSharpHtmlDocument;
            int? views = null;
            var viewsText = angleSharpHtmlDocument.QuerySelector(".document-metrics:nth-child(3)")?.Text();
            if (!string.IsNullOrEmpty(viewsText))
            {
                if (int.TryParse(Regex.Match(viewsText, @"\d+").Value, out int realViews))
                {
                    views = realViews;
                }
            }
            //var contentCountText = angleSharpHtmlDocument.QuerySelector("span.document-metrics")?.Text()?.Trim();
            //int.TryParse(Regex.Match(contentCountText ?? "", @"\d+").Value, out int contentCount);
            //var title = angleSharpHtmlDocument.QuerySelector("span.current")?.Text()?.Trim() ?? angleSharpHtmlDocument.QuerySelector("title")?.Text()?.Trim();

            var metaDescription = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[name=description]")?.Content?.Trim();
            var metaKeyword = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[name=keywords]")?.Content?.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var metaImage = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content?.Trim();
            string university = null, course = null, content = null;
            string[] tags = { };
            var allHeaders = angleSharpHtmlDocument.QuerySelectorAll(".small-padding-bottom.detail-box h5");
            DateTime? createDate = null;
            //Init the extra data details according what exist
            foreach (var item in allHeaders)
            {
                var header = item.ChildNodes[0].Text().Trim();
                if (header.Equals("School:")) { university = item.FirstElementChild.Text(); }
                else if (header.Equals("Course:")) { course = item.FirstElementChild.Text(); }
                else if (header.Equals("Tags:")) { tags = item.FirstElementChild.Text()?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries); }
                else if (header.Equals("Upload Date:"))
                {
                    var dateString = item.FirstElementChild.Text().Replace("  ", " ");
                    if (DateTime.TryParseExact(dateString, "ddd MMM d HH:mm:ss yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime realCreateDate))
                    {
                        createDate = realCreateDate;
                    }
                }
            }

            if (!page.Uri.AbsolutePath.StartsWith("/flashcard"))
            {
                content = angleSharpHtmlDocument.QuerySelector("#material_text")?.Text()?.Trim();
            }
            else
            {
                var spaceReg = new Regex(@"\s+", RegexOptions.Compiled);
                //Remove the cards front back headers from the text
                var slides = angleSharpHtmlDocument.QuerySelectorAll<IHtmlDivElement>("#preview>div:first-child .row");
                if (slides != null)
                {
                    var builder = new StringBuilder();
                    foreach (var slide in slides)
                    {
                        if (slide.ClassList.Length > 1)
                        {
                            continue;
                        }
                        var txt = spaceReg.Replace(slide.Text(), " ");
                        builder.Append(txt);
                    }
                    content = builder.ToString();
                }
                //if (allContent != null)
                //{
                //    var backIndex = allContent.IndexOf("Back", StringComparison.Ordinal);
                //    content = allContent.Substring(backIndex + "Back".Length).Trim();
                //}
            }
            return new CrawlModel(page.Uri.AbsoluteUri, angleSharpHtmlDocument.Title, content, university, course,
                tags, createDate, views, metaDescription, metaImage, metaKeyword, page.Uri.Host, CalculateMd5Hash(page.Uri.AbsoluteUri));

        }


        public static string CalculateMd5Hash(string input)

        {
            // step 1, calculate MD5 hash from input
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hash = md5.ComputeHash(inputBytes);
                // step 2, convert byte array to hex string
                var sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }

        }

        public void Dispose()
        {
            m_Crawler?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync, CancellationToken token)
        {
            var studySoupSiteMap = new Uri("https://studysoup.com/sitemap.xml.gz");
            var t = m_Crawler.CrawlAsync(studySoupSiteMap);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), token).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {

                }
                finally
                {
                    if (token.IsCancellationRequested)
                    {
                        m_Crawler.Stop();
                    }
                }


            }
            await t.ConfigureAwait(false);
            return true;
        }
    }
}
