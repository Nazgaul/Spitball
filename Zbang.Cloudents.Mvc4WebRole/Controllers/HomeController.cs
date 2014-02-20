using DevTrends.MvcDonutCaching;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using System.Web.WebPages;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using System.Threading.Tasks;
using System.Resources;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using Zbang.Cloudents.Mvc4WebRole.Models.FAQ;
using Zbang.Zbox.Infrastructure.Cache;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Web.UI;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : BaseController
    {

        private readonly Lazy<IQueueProvider> m_QueueProvider;
        private readonly Lazy<IBlobProvider> m_BlobProvider;
        private readonly Lazy<ICache> m_CahceProvider;

        public HomeController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            // IShortCodesCache shortToLongCache,
            Lazy<IQueueProvider> queueProvider,
            Lazy<IBlobProvider> blobProvider,
            Lazy<ICache> cacheProvider,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService,
                // shortToLongCache, 
            formsAuthenticationService)
        {
            m_QueueProvider = queueProvider;
            m_BlobProvider = blobProvider;
            m_CahceProvider = cacheProvider;
        }

        //[ZboxAuthorize]
        [NoUniversityAttribute]
        public ActionResult Index(long? universityId)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToActionPermanent("Index", "Dashboard");
            }
            return RedirectToAction("Index", "Account", new { universityId = universityId });
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
            var model = m_CahceProvider.Value.GetFromCache(faqQuestionCacheName, faqQuestionCacheName) as IEnumerable<Category>;

            if (model != null)
            {
                return View(model.Where(w => w.Language.ToLower() == System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower()));

            }
            using (var stream = await m_BlobProvider.Value.GetFAQQeustion())
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
                m_CahceProvider.Value.AddToCache(faqQuestionCacheName, model.ToList(), TimeSpan.FromHours(1), faqQuestionCacheName);
            }
            return View(model.Where(w => w.Language.ToLower() == System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower()));

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
                TraceLog.WriteError("On Statistics" + model.ToString(), ex);
                return Json(new JsonResponse(true));
            }
        }


        [DonutOutputCache(Duration = TimeConsts.Hour,
            VaryByParam = "none",
            VaryByCustom = CustomCacheKeys.Lang, Order = 2)]
        [CompressFilter(Order = 1)]
        public ActionResult JsResources()
        {
            ResourceManager rm = new ResourceManager("Zbang.Cloudents.Mvc4WebRole.Js.Resources.JsResources", Assembly.GetExecutingAssembly());
            var set = rm.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentUICulture, true, true);

            var sb = new StringBuilder();

            sb.Append("JsResources={");
            foreach (System.Collections.DictionaryEntry item in set)
            {
                sb.Append("\"" + item.Key.ToString() + "\":\"" + item.Value.ToString().Replace("\n", @"\n") + "\",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return Content(sb.ToString(), "application/javascript");
        }
        [AllowAnonymous]
        [HttpGet]
        [OutputCache(Duration = 24 * 60 * 60, Location = System.Web.UI.OutputCacheLocation.Any)]
        public async Task<ActionResult> SiteMap()
        {
            var content = await GetSitemapXml();
            return Content(content, "application/xml", Encoding.UTF8);
        }

        [NonAction]
        private async Task<string> GetSitemapXml()
        {
            const string SitemapsNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root;
            XNamespace xmlns = SitemapsNamespace;

            var nodes = await GetSitemapNodes();

            root = new XElement(xmlns + "urlset");


            foreach (var node in nodes)
            {
                root.Add(
                new XElement(xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(node.Url)),
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
        private async Task<IEnumerable<SitemapNode>> GetSitemapNodes()
        {
            var urlBuilder = new UrlBuilder(HttpContext);
            var requestContext = ControllerContext.RequestContext;
            List<SitemapNode> nodes = new List<SitemapNode>
            {
                new SitemapNode(requestContext, new { area = "", controller = "Home", action = "Index" })
                {
                    Priority = 1.0,
                    Frequency = SitemapFrequency.Daily
                },
                 new SitemapNode(requestContext, new { area = "", controller = "Home", action = "AboutUs" })
                {
                    Priority = 0.95,
                    Frequency = SitemapFrequency.Daily
                },
                  new SitemapNode(requestContext, new { area = "", controller = "Home", action = "ContactUs" })
                {
                    Priority = 0.95,
                    Frequency = SitemapFrequency.Daily
                },
                  new SitemapNode(requestContext, new { area = "", controller = "Home", action = "Privacy" })
                {
                    Priority = 0.8,
                    Frequency = SitemapFrequency.Daily
                },
                  new SitemapNode(requestContext, new { area = "", controller = "Home", action = "TermsOfService" })
                {
                    Priority = 0.8,
                    Frequency = SitemapFrequency.Daily
                },
                //  new SitemapNode(urlBuilder.BuildBoxUrl(12,"test on study","test"),ControllerContext.RequestContext)
                //{
                //    Priority = 0.8,
                //    Frequency = SitemapFrequency.Daily
                //}


            };

            //nodes.Add(new SitemapNode(this.ControllerContext.RequestContext, new { area = "", controller = "Home", action = "Index" })
            //{
            //    Frequency = SitemapFrequency.Always,
            //    Priority = 0.8
            //});
            var seoItems = await m_ZboxReadService.GetSeoBoxesAndItems();
            foreach (var box in seoItems.Boxes)
            {
                nodes.Add(new SitemapNode(urlBuilder.BuildBoxUrl(box.Id, box.Name, box.UniversityName), requestContext));
            }
            foreach (var item in seoItems.Items.Take(40000))
            {
                nodes.Add(new SitemapNode(urlBuilder.buildItemUrl(item.BoxId, item.BoxName, item.Id, item.Name, item.UniversityName), requestContext));
            }
            //var items = Query(new GetSeoContentPages(false));
            //foreach (var item in items)
            //{
            //    nodes.Add(new SitemapNode(this.ControllerContext.RequestContext, new { area = "", controller = "Page", action = "ContentPage", id = item.Slug })
            //    {
            //        Frequency = SitemapFrequency.Yearly,
            //        Priority = 0.5,
            //        LastModified = item.Modified
            //    });
            //}

            return nodes;
        }
    }
}
