using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using AbotX.Core;
using AbotX.Crawler;
using AbotX.Poco;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Enums;
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
        private readonly ILogger _logger;

        public Crawler(IBlobProvider2<CrawlContainerName> blobProvider,
            IDocumentDbRepository<CrawlModel> documentDbRepository, IMailComponent mailManager, ILogger logger)
        {
            m_BlobProvider = blobProvider;
            m_DocumentDbRepository = documentDbRepository;
            m_MailManager = mailManager;
            _logger = logger;
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

        private void CrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            var crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                _logger.Warning(
                    $"Crawl of page failed {crawledPage.Uri.AbsoluteUri} , exception {crawledPage.WebException}");
                return;
            }

            if (!crawledPage.HttpWebResponse.ContentType.Contains("text/html"))
            {
                _logger.Warning(
                    $"Crawl of page {crawledPage.Uri.AbsoluteUri} of type {crawledPage.HttpWebResponse.ContentType}");
                return;
            }

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
            {
                _logger.Warning($"Page had no content {crawledPage.Uri.AbsoluteUri}");
                return;
            }
            //Create dictionary of Authority and related create model function
            var validAuth = new Dictionary<string, string>
            {
                ["studysoup.com"] = "CreateStudySoupNote",
                ["www.khanacademy.org"] = "CreateKhananNote",
                ["quizlet.com"] = "CreateQuizletFlashcard",
                ["www.studyblue.com"] = "CreateStudyBlueFlashcard"
            };
            //Use reflection to decide which mapping to run
            if (validAuth.TryGetValue(crawledPage.Uri.Authority, out string modelFunction))
            {
                try
                {
                    var model = (CrawlModel)GetType()
                        .GetMethod(modelFunction, BindingFlags.Static | BindingFlags.NonPublic)
                        .Invoke(null, new[] { crawledPage });
                    var str = JsonConvert.SerializeObject(model);
                    m_BlobProvider.UploadText(model.Id, str);
                    m_DocumentDbRepository.CreateItemAsync(model).Wait();
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex, new Dictionary<string, string> { ["page"] = crawledPage.ToString() });
                }
            }
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.Info("starting to log");
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
                        m_Crawler.Stop();
                }
            }

            await t.ConfigureAwait(false);
            var result = t.Result;
            if (result.ErrorOccurred)
            {
                await m_MailManager.GenerateSystemEmailAsync("crawler",
                        $"Crawl of {result.RootUri.AbsoluteUri} completed with error: {result.ErrorException.Message}")
                    .ConfigureAwait(false);
                _logger.Error(
                    $"Crawl of {result.RootUri.AbsoluteUri} completed with error: {result.ErrorException.Message}");
            }
            else
            {
                await m_MailManager.GenerateSystemEmailAsync("crawler",
                    $"Crawl of {result.RootUri.AbsoluteUri} completed without error.").ConfigureAwait(false);
                _logger.Info($"Crawl of {result.RootUri.AbsoluteUri} completed without error.");
            }
        }

        public string Name => nameof(Crawler);

        //Khanan Academy crawler
        private static CrawlModel CreateKhananNote(CrawledPage page)
        {
            const string invalidCdnUrl = "https://cdn.kastatic.org/googleusercontent/K5bzbA067FpSFjs7VuTCAEosuCGLm4NfxQbq_tYtpMHIyB5j-nirP_Pdy8XXrmoARE3_2TBnGafYaRTsSiFt4iw";
            var doc = page.AngleSharpHtmlDocument;
            var metaDescription = doc.QuerySelector<IHtmlMetaElement>("meta[name=description]")?.Content?.Trim();
            var metaImage = doc.QuerySelector("[rel=image_src]")?.GetAttribute("href") ??
                            doc.QuerySelector<IHtmlMetaElement>("meta[name=thumbnail]")?.Content?.Trim() ??
                            doc.QuerySelector("[role=tabpanel] .fixed-to-responsive img")?.GetAttribute("src") ??
                            doc.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content?.Trim();
            var metaKeyword = doc.QuerySelector<IHtmlMetaElement>("meta[name=keywords]")?.Content?.Trim()
                .Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);
            var tags = doc.QuerySelector<IHtmlMetaElement>("meta[name='sailthru.tags']")?.Content?.Trim()
                .Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);
            //Check if the image is cdn not exist change to default
            if (!string.IsNullOrEmpty(metaImage) && metaImage.Equals(invalidCdnUrl))
                metaImage = doc.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content?.Trim();
            var content = doc.QuerySelector(".perseus-renderer")?.Text()?.Trim() ?? doc
                              .QuerySelector(".task-container:not([itemtype*=Video]),[class^=tabContent]")?.Text()
                              ?.Trim() ??
                          doc.QuerySelector("[class^=description],[class^=module]")?.TextContent?.Trim();
            //split in case lowerCase close to UpperCase
            if (!string.IsNullOrEmpty(content)) content = Regex.Replace(content, "([a-z])([A-Z])", "$1 $2");

            return new CrawlModel(page.Uri.AbsoluteUri, doc.Title, content, null, null,
                tags, null, null, metaDescription, metaImage, metaKeyword, page.Uri.Host,
                MD5HashGenerator.GenerateKey(page.Uri.AbsoluteUri), ItemType.Document);
        }

        private static CrawlModel CreateQuizletFlashcard(CrawledPage page)
        {
            var doc = page.AngleSharpHtmlDocument;
            var metaDescription = doc.QuerySelector<IHtmlMetaElement>("meta[name=description]")?.Content?.Trim();
            var allCards = doc.QuerySelectorAll(".SetPage-term").Select(s => string.IsNullOrEmpty(s?.TextContent)
                ? string.Empty
                : Regex.Replace(s?.TextContent?.Trim(), "([a-z])([A-Z])", "$1 $2", RegexOptions.CultureInvariant));
            var enumerable = allCards as string[] ?? allCards.ToArray();
            if (enumerable.Length == 0)
                return null;
            var list = enumerable?.ToList();
            var firstDefImage = doc.QuerySelector(".SetPageTerm-image")?.GetAttribute("style");
            var logoImg = doc.QuerySelector("img")?.GetAttribute("src");
            var image = !(string.IsNullOrEmpty(firstDefImage)) ? Regex.Match(firstDefImage, @"(?<=\().+?(?=\))").Value :
                (!string.IsNullOrEmpty(logoImg) ? (page.ParentUri.OriginalString + logoImg) : null);
            list.RemoveAll(string.IsNullOrWhiteSpace);
            var content = list.Count > 0 ? list.Aggregate<string>((a, b) => a + ", " + b) : null;
            return new CrawlModel(page.Uri.AbsoluteUri, page.Uri.Host, doc.Title, content, metaDescription, image,
                MD5HashGenerator.GenerateKey(page.Uri.AbsoluteUri));
        }

        private static CrawlModel CreateStudyBlueFlashcard(CrawledPage page)
        {
            var doc = page.AngleSharpHtmlDocument;
            var metaDescription = doc.QuerySelector<IHtmlMetaElement>("meta[name=description]")?.Content?.Trim();
            var allCards = doc.QuerySelectorAll("[id^=card].card").Select(s => string.IsNullOrEmpty(s?.TextContent)
                ? string.Empty
                : s?.TextContent?.Trim());
            //Validate that have value(no sitemap)
            var enumerable = allCards as string[] ?? allCards.ToArray();
            if (enumerable.Length == 0)
                return null;
            var list = enumerable?.ToList();
            var metaImg = doc.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content?.Trim();
            list.RemoveAll(string.IsNullOrWhiteSpace);
            //Remove extra spaces 
            var spaceReg = new Regex(@"\s+", RegexOptions.Compiled);
            var content = list.Count > 0 ? spaceReg.Replace(list.Aggregate((a, b) => a + ", " + b), " ") : null;
            return new CrawlModel(page.Uri.AbsoluteUri, page.Uri.Host, doc.Title, content, metaDescription, metaImg,
                MD5HashGenerator.GenerateKey(page.Uri.AbsoluteUri));
        }

        private static CrawlModel CreateStudySoupNote(CrawledPage page)
        {
            var angleSharpHtmlDocument = page.AngleSharpHtmlDocument;
            int? views = null;
            var viewsText = angleSharpHtmlDocument.QuerySelector(".document-metrics:nth-child(3)")?.Text();
            if (!string.IsNullOrEmpty(viewsText)
                && int.TryParse(Regex.Match(viewsText, @"\d+").Value, out int realViews))
            {
                views = realViews;
            }

            var metaDescription = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[name=description]")
                ?.Content?.Trim();
            var metaKeyword = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[name=keywords]")?.Content
                ?.Trim().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var metaImage = angleSharpHtmlDocument.QuerySelector<IHtmlMetaElement>("meta[property='og:image']")?.Content
                ?.Trim();
            string university = null, course = null, content = null;
            string[] tags = { };
            var type = ItemType.Document;
            var allHeaders = angleSharpHtmlDocument.QuerySelectorAll(".small-padding-bottom.detail-box h5");
            DateTime? createDate = null;
            //Init the extra data details according what exist
            foreach (var item in allHeaders)
            {
                var header = item.ChildNodes[0].Text().Trim();
                if (header.Equals("School:"))
                {
                    university = item.FirstElementChild.Text();
                }
                else if (header.Equals("Course:"))
                {
                    course = item.FirstElementChild.Text();
                }
                else if (header.Equals("Tags:"))
                {
                    tags = item.FirstElementChild.Text()?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                }
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
                var contentSplit = angleSharpHtmlDocument.QuerySelector("#material_text")?.TextContent?.Split('\n');
                content = contentSplit.Length > 3 ? contentSplit[2]?.Trim() : contentSplit[0]?.Trim();

                //Improve default image if possible
                var firstPageImg = angleSharpHtmlDocument.QuerySelector(".page_number_1");
                if (firstPageImg != null)
                {
                    var index = firstPageImg.GetAttribute("style").IndexOf("url(");
                    metaImage = firstPageImg.OuterHtml.Substring(index + "url(".Length).Split(')')[0];
                }
            }
            else
            {
                type = ItemType.Flashcard;
                var spaceReg = new Regex(@"\s+", RegexOptions.Compiled);
                //Remove the cards front back headers from the text
                var slides = angleSharpHtmlDocument.QuerySelectorAll<IHtmlDivElement>("#preview>div:first-child .row");
                if (slides != null)
                {
                    var builder = new StringBuilder();
                    foreach (var slide in slides)
                    {
                        if (slide.ClassList.Length > 1)
                            continue;
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
                tags, createDate, views, metaDescription, metaImage, metaKeyword, page.Uri.Host,
                MD5HashGenerator.GenerateKey(page.Uri.AbsoluteUri), type);
        }

        public void Dispose()
        {
            m_Crawler?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync,
            CancellationToken token)
        {
            _logger.Info("starting to log");
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
                        m_Crawler.Stop();
                }
            }

            await t.ConfigureAwait(false);
            return true;
        }
    }
}
