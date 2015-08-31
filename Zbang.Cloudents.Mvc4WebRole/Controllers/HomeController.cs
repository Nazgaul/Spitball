﻿using System.Web.UI;
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
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Cloudents.Mvc4WebRole.Models.FAQ;
using Zbang.Cloudents.SiteExtension;
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
        private readonly Lazy<IBlobProvider> m_BlobProvider;
        private readonly Lazy<ICache> m_CacheProvider;
        private readonly Lazy<IQueueProvider> m_QueueProvider;

        public HomeController(
            Lazy<IBlobProvider> blobProvider,
            Lazy<ICache> cacheProvider, Lazy<IQueueProvider> queueProvider)
        {
            m_BlobProvider = blobProvider;
            m_CacheProvider = cacheProvider;
            m_QueueProvider = queueProvider;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", User.Identity.IsAuthenticated ? "Dashboard" : "Account");
        }

        //don't put in here route attribute
        [DonutOutputCache(Duration = 3600,
            VaryByParam = "None",
            VaryByCustom = CustomCacheKeys.Auth + ";"
            + CustomCacheKeys.Lang)]
        [RedirectToMobile(Order = 1)]
        //this is for search,library choose,account settings home page
        public ActionResult IndexEmpty()
        {
            return View("Empty");
        }

        //public ActionResult Redirect()
        //{
        //    return RedirectToAction("Index", User.Identity.IsAuthenticated ? "Dashboard" : "Account");
        //}

        [DonutOutputCache(Duration = TimeConsts.Day, VaryByParam = "None", VaryByCustom = CustomCacheKeys.Auth + ";"
            + CustomCacheKeys.Lang)]
        public ActionResult ContactUs()
        {

            return View();
        }


        public ActionResult TestPage()
        {

            return View();
        }

        [DonutOutputCache(CacheProfile = "FullPage")]
        public ActionResult TermsOfService()
        {
            return View();
        }

        [DonutOutputCache(CacheProfile = "FullPage")]
        public ActionResult Privacy()
        {
            return View();
        }

        [DonutOutputCache(Duration = TimeConsts.Minute * 5, VaryByParam = "None", VaryByCustom = CustomCacheKeys.Auth + ";"
           + CustomCacheKeys.Lang)]
        [Route("jobs")]
        public async Task<ActionResult> Jobs()
        {
            ViewBag.jobs = "jobs";

            using (var stream = await m_BlobProvider.Value.GetJobsXml())
            {
                if (stream == null)
                {
                    TraceLog.WriteError("jobs xml stream is null");
                    return RedirectToAction("Index", "Error");
                }
                var data = XDocument.Load(stream);
                var retVal = data.Descendants("job").Select(s => new Job
                {
                    Name = s.Attribute("name").Value,
                    Location = s.Attribute("location").Value,
                    Description = s.Element("description").Value,
                });
                return View(retVal);
            }
        }

        public async Task<ActionResult> Help()
        {
            const string viewName = "help2";
            const string faqQuestionCacheName = "faqQuestionCacheName";
            var model = await m_CacheProvider.Value.GetFromCacheAsync<IEnumerable<Category>>(faqQuestionCacheName, faqQuestionCacheName);

            if (model != null)
            {
                return View(viewName,model.Where(w => String.Equals(w.Language,
                    System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, StringComparison.CurrentCultureIgnoreCase)));

            }
            using (var stream = await m_BlobProvider.Value.GetFaqQuestion())
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
                await m_CacheProvider.Value.AddToCacheAsync(faqQuestionCacheName, model, TimeSpan.FromHours(1), faqQuestionCacheName);
            }
            return View(viewName, model.Where(w => String.Equals(w.Language,
                System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, StringComparison.CurrentCultureIgnoreCase)));

        }
        [DonutOutputCache(CacheProfile = "FullPage")]
        public ActionResult AboutUs()
        {
            return View();
        }

        [DonutOutputCache(CacheProfile = "FullPage")]
        public ActionResult IFrame()
        {
            return View();
        }

        //public ActionResult MobileApp()
        //{
        //    return View();
        //}

        //public ActionResult CloudentsIsNowSpitball()
        //{
        //    return PartialView();
        //}

        [ChildActionOnly]
        public ActionResult AntiForgeryToken()
        {
            return PartialView("_AntiForgeryToken");
        }



        [DonutOutputCache(Duration = TimeConsts.Day,
            VaryByParam = "none", Location = OutputCacheLocation.Server,
            VaryByCustom = CustomCacheKeys.Lang, Order = 2)]
        [CacheFilter(Duration = TimeConsts.Day)]
        public ActionResult JsResources()
        {
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
        }


        [AllowAnonymous]
        [HttpGet]
        [OutputCache(Duration = 2 * TimeConsts.Day, VaryByParam = "index", Location = OutputCacheLocation.Any)]
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
                   new SitemapNode(requestContext, new { area = "", controller = "Home", action = "Jobs" })
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
            nodes.AddRange(seoItems.Where(w => !string.IsNullOrEmpty(w))
                .Select(s => new SitemapNode(s, requestContext)));

            return nodes;
        }


        [ZboxAuthorize]
        public ActionResult InsertUser()
        {
            var universityId = long.Parse(ConfigFetcher.Fetch("StudentUnionToAddId"));
            if (User.GetUserId() != universityId)
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
            if (User.GetUserId() != universityId)
            {
                return RedirectToAction("index");
            }
            var command = new AddStudentCommand(model.Id);
            ZboxWriteService.AddStudent(command);
            return RedirectToAction("InsertUser", new { complete = "complete" });
        }

        [HttpGet]
        public async Task<JsonResult> Version()
        {
            if (User.Identity.IsAuthenticated)
            {
                await m_QueueProvider.Value.InsertMessageToTranactionAsync(
                      new StatisticsData4(null, User.GetUserId(), DateTime.UtcNow));
            }
            return JsonOk(VersionHelper.CurrentVersion(true));
        }
    }
}
