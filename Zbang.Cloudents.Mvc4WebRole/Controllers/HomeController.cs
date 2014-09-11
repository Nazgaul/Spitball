using System.Web.UI;
using DevTrends.MvcDonutCaching;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Cloudents.Mvc4WebRole.Models.FAQ;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : BaseController
    {
        private readonly Lazy<IQueueProvider> m_QueueProvider;
        private readonly Lazy<IBlobProvider> m_BlobProvider;
        private readonly Lazy<ICache> m_CacheProvider;

        public HomeController(
            Lazy<IQueueProvider> queueProvider,
            Lazy<IBlobProvider> blobProvider,
            Lazy<ICache> cacheProvider
            )
        {
            m_QueueProvider = queueProvider;
            m_BlobProvider = blobProvider;
            m_CacheProvider = cacheProvider;
        }

        //[ZboxAuthorize]
        [NoUniversity]
        [NonAjax]
        [NoCache]
        //[UserNavNWelcome]
        //[Route("box:desktop/my/{boxId:long}/{boxName}", Name = "PrivateBoxDesktop", Order = 1)]
        //[Route("course:desktop/{universityName}/{boxId:long}/{boxName}", Name = "CourseBoxDesktop", Order = 2)]
        public ActionResult Index(long? universityId)
        {
            if (!User.Identity.IsAuthenticated && Request.UserAgent != null &&
                !Request.UserAgent.ToUpper().Contains("MSIE 9.0"))
            {
                return RedirectToAction("Index", "Account", new { universityId });
                //return RedirectToActionPermanent("Index", "Dashboard");
            }
            //this is the only place we need
            if (DisplayConfig.CheckIfMobileView(HttpContext))
            {
                return RedirectToActionPermanent("Index", "Dashboard");
            }

            return View("Empty");
        }

        [DonutOutputCache(Duration = TimeConsts.Day, VaryByParam = "None", VaryByCustom = CustomCacheKeys.Auth + ";"
            + CustomCacheKeys.Lang)]
        public ActionResult ContactUs()
        {

            return View();
        }

        [DonutOutputCache(Duration = TimeConsts.Day, VaryByParam = "None", VaryByCustom = CustomCacheKeys.Auth + ";"
            + CustomCacheKeys.Lang)]
        public ActionResult TermsOfService()
        {
            return View();
        }

        [DonutOutputCache(Duration = TimeConsts.Day, VaryByParam = "None", VaryByCustom = CustomCacheKeys.Auth + ";"
            + CustomCacheKeys.Lang)]
        public ActionResult Privacy()
        {
            return View();
        }
        public async Task<ActionResult> Help()
        {
            const string faqQuestionCacheName = "faqQuestionCacheName";
            var model = m_CacheProvider.Value.GetFromCache(faqQuestionCacheName, faqQuestionCacheName) as IEnumerable<Category>;

            if (model != null)
            {
                return View(model.Where(w => String.Equals(w.Language,
                    System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, StringComparison.CurrentCultureIgnoreCase)));

            }
            using (var stream = await m_BlobProvider.Value.GetFaqQeustion())
            {
                var data = XDocument.Load(stream);
                model = from category in data.Descendants("category")
                        let faqs = category.Descendants("content")
                        orderby int.Parse(category.Attribute("order").Value)
                        select new Category
                        {
                            Language = category.Attribute("lang").Value,
                            Name = category.Attribute("name").Value,
                            Order = int.Parse(category.Attribute("order").Value),
                            QuestionNAnswers = faqs.Select(s =>
                                new QnA
                                {
                                    Answer = s.Element("answer").Value,
                                    Question = s.Element("question").Value,
                                    Order = int.Parse(s.Attribute("order").Value)

                                }).OrderBy(s => s.Order).ToList()
                        };
                model = model.ToList();
                m_CacheProvider.Value.AddToCache(faqQuestionCacheName, model, TimeSpan.FromHours(1), faqQuestionCacheName);
            }
            return View(model.Where(w => String.Equals(w.Language,
                System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, StringComparison.CurrentCultureIgnoreCase)));

        }
        [DonutOutputCache(Duration = TimeConsts.Day, VaryByParam = "None", VaryByCustom = CustomCacheKeys.Auth + ";"
            + CustomCacheKeys.Lang)]
        public ActionResult AboutUs()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult AntiForgeryToken()
        {
            return PartialView("_AntiForgeryToken");
        }


        [Ajax, HttpPut]
        public async Task<ActionResult> Statistics(Statistics model)
        {
            try
            {
                if (model == null)
                {
                    model = new Statistics();
                }
                if (model.Items == null)
                {
                    model.Items = new StatisticItem[0];
                }
                await m_QueueProvider.Value.InsertMessageToTranactionAsync(new StatisticsData4(model.Items.Select(s =>
                      new StatisticsData4.StatisticItemData { Id = s.Uid, Action = (int)s.Action }), GetUserId(false), DateTime.UtcNow));

                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                if (model != null) TraceLog.WriteError("On Statistics" + model, ex);
                return Json(new JsonResponse(true));
            }
        }


        [DonutOutputCache(Duration = TimeConsts.Day,
            VaryByParam = "none", Location = OutputCacheLocation.Server,
            VaryByCustom = CustomCacheKeys.Lang, Order = 2)]
        [CacheFilter(Duration = TimeConsts.Day)]
        public ActionResult JsResources()
        {
            //var rm = new ResourceManager("Zbang.Cloudents.Mvc4WebRole.Js.Resources.JsResources", Assembly.GetExecutingAssembly());
            var x = typeof(Js.Resources.JsResources);
            var sb = new StringBuilder();
            sb.Append("JsResources={");
            foreach (var p in x.GetProperties())
            {

                var s = p.GetValue(null, null);
                if (s is string)
                {
                    sb.Append("\"" + p.Name + "\":\"" +
                              s.ToString().Replace("\r\n", @"\n").Replace("\n", @"\n").Replace("\"", @"\""") +
                              "\",");
                    sb.AppendLine();
                }


            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return Content(sb.ToString(), "application/javascript");
            //using (var set = rm.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentUICulture, true, true))
            //{

            //    var sb = new StringBuilder();

            //    sb.Append("JsResources={");
            //    foreach (System.Collections.DictionaryEntry item in set)
            //    {

            //        sb.Append("\"" + item.Key + "\":\"" +
            //                  item.Value.ToString().Replace("\r\n", @"\n").Replace("\n", @"\n").Replace("\"", @"\""") +
            //                  "\",");
            //        sb.AppendLine();
            //    }
            //    sb.Remove(sb.Length - 1, 1);
            //    sb.Append("}");
            //    return Content(sb.ToString(), "application/javascript");
            //}
        }


        [AllowAnonymous]
        [HttpGet]
        [OutputCache(Duration = TimeConsts.Day, VaryByParam = "index", Location = OutputCacheLocation.Any)]
        public async Task<ActionResult> SiteMap(int? index)
        {
            if (!index.HasValue)
            {
                var contentIndex = await GetSitemapIndex();
                return Content(contentIndex, "application/xml", Encoding.UTF8);
            }
            var content = await GetSitemapXml(index.Value);
            return Content(content, "application/xml", Encoding.UTF8);
        }

        private async Task<string> GetSitemapIndex()
        {
            const string sitemapsNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace xmlns = sitemapsNamespace;
            var noOfSiteMaps = await ZboxReadService.GetSeoItemCount();

            var root = new XElement(xmlns + "sitemapindex");
            for (int i = 1; i <= noOfSiteMaps; i++)
            {
                root.Add(
                    new XElement(xmlns + "sitemap",
                        new XElement(xmlns + "loc", string.Format("https://www.cloudents.com/sitemap-{0}.xml", i))
                           )
                        );

            }
            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms, Encoding.UTF8))
                {
                    root.Save(writer);
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }


        }

        [NonAction]
        private async Task<string> GetSitemapXml(int index)
        {
            const string sitemapsNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace xmlns = sitemapsNamespace;

            var nodes = await GetSitemapNodes(index);

            var root = new XElement(xmlns + "urlset");



            foreach (var node in nodes)
            {
                root.Add(
                new XElement(xmlns + "url",
                    new XElement(xmlns + "loc", node.Url),
                    node.Priority == null ? null : new XElement(xmlns + "priority", node.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)),
                    node.LastModified == null ? null : new XElement(xmlns + "lastmod", node.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    node.Frequency == null ? null : new XElement(xmlns + "changefreq", node.Frequency.Value.ToString().ToLowerInvariant())
                    ));
            }

            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms, Encoding.UTF8))
                {
                    root.Save(writer);
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        [NonAction]
        private async Task<IEnumerable<SitemapNode>> GetSitemapNodes(int index)
        {
            var requestContext = ControllerContext.RequestContext;
            var nodes = new List<SitemapNode>();
            if (index == 1)
            {
                nodes.Add(
                new SitemapNode(requestContext, new { area = "", controller = "Home", action = "Index" })
                {
                    Priority = 1.0,
                    Frequency = SitemapFrequency.Daily
                });
                nodes.Add(
                    new SitemapNode("/account/he-il/", requestContext)
                    {
                        Priority = 1.0,
                        Frequency = SitemapFrequency.Daily
                    });
                nodes.Add(
                    new SitemapNode("/account/ru-ru/", requestContext)
                    {
                        Priority = 1.0,
                        Frequency = SitemapFrequency.Daily
                    });
                nodes.Add(
                    new SitemapNode(requestContext, new { area = "", controller = "Home", action = "AboutUs" })
                    {
                        Priority = 0.95,
                        Frequency = SitemapFrequency.Daily
                    });
                nodes.Add(
                    new SitemapNode(requestContext, new { area = "", controller = "Home", action = "ContactUs" })
                    {
                        Priority = 0.95,
                        Frequency = SitemapFrequency.Daily
                    });
                nodes.Add(
                    new SitemapNode(requestContext, new { area = "", controller = "Home", action = "Privacy" })
                    {
                        Priority = 0.8,
                        Frequency = SitemapFrequency.Daily
                    });
                nodes.Add(
                    new SitemapNode(requestContext, new { area = "", controller = "Home", action = "TermsOfService" })
                    {
                        Priority = 0.8,
                        Frequency = SitemapFrequency.Daily
                    });
            }

            var seoItems = await ZboxReadService.GetSeoItems(index);
            nodes.AddRange(seoItems.Select(box => new SitemapNode(box, requestContext)));

            return nodes;
        }


        //[OutputCache(Duration = TimeConsts.Day, VaryByParam = "none")]
        //public ActionResult Bootstrap()
        //{
        //    var routes = Server.MapPath("~/Js/bootstrap.js");
        //    var str = System.IO.File.ReadAllText(routes);

        //    //var jsFileLocations = BundleConfig.JsRemoteLinks();
        //    var matches = Regex.Matches(str, @"\{(.*?)\}");
        //    foreach (Match match in matches)
        //    {
        //        var matchWithoutBrackets = match.Value.Replace("{", string.Empty).Replace("}", string.Empty);
        //        var filesToFind = matchWithoutBrackets.Split('-');
        //        var jsFileLocation = BundleConfig.JsRemoteLinks(filesToFind[0]);
        //        var files = jsFileLocation.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        //        var retVal = files[0];
        //        if (filesToFind.Length == 2)
        //        {

        //            retVal = files.FirstOrDefault(
        //                f =>
        //                    String.Equals(f.Trim().Replace(".js", string.Empty), filesToFind[1],
        //                        StringComparison.CurrentCultureIgnoreCase))
        //                         ?? files[0];
        //        }

        //        str = str.Replace(match.Value, retVal.Replace(".js", string.Empty).Trim());


        //    }
        //    //foreach (var jsFileLocation in jsFileLocations)
        //    //{

        //    //    var files = jsFileLocation.Value.Split(new [] { "," }, StringSplitOptions.RemoveEmptyEntries);

        //    //    matches.
        //    //    //var jsFiles = jsFileLocation.Value.Trim();

        //    //    //sb.Replace("{" + jsFileLocation.Key + "}", jsFiles);
        //    //}
        //    if (!Request.IsLocal)
        //    {
        //        var minifer = new Minifier();
        //        str = minifer.MinifyJavaScript(str);
        //    }
        //    return Content(str, "application/javascript");
        //}


        [ZboxAuthorize]
        public ActionResult InsertUser()
        {
            var universityId = long.Parse(ConfigFetcher.Fetch("StudentUnionToAddId"));
            if (GetUserId() != universityId)
            {
                return RedirectToAction("index");
            }
            return View(new InsertUser());
        }

        [ZboxAuthorize]
        [HttpPost]
        public ActionResult InsertUser(InsertUser model)
        {
            var universityId = long.Parse(ConfigFetcher.Fetch("StudentUnionToAddId"));
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (GetUserId() != universityId)
            {
                return RedirectToAction("index");
            }
            var command = new AddStudentCommand(model.Id);
            ZboxWriteService.AddStudent(command);
            return RedirectToAction("InsertUser", new { complete = "complete" });
        }
    }
}
