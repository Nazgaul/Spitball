using System.Threading;
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
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Cloudents.Mvc4WebRole.Models.Account;
using Zbang.Cloudents.Mvc4WebRole.Models.FAQ;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : BaseController
    {
        private readonly Lazy<IBlobProvider> m_BlobProvider;
        private readonly ICookieHelper m_CookieHelper;
        public static readonly long[] FlashcardUniversities = { 173408, 171885, 172566 };

        public HomeController(Lazy<IBlobProvider> blobProvider, ICookieHelper cookieHelper)
        {
            m_BlobProvider = blobProvider;
            m_CookieHelper = cookieHelper;
        }

        [UniversityCookieInject("universityName", Order = 1)]
        [DonutOutputCache(CacheProfile = "HomePage", Order = 2), ActionName("Index"), HttpGet]
        public async Task<ActionResult> IndexAsync(string invId, string universityName, string step)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.title = SeoResources.HomePageTitle;
            ViewBag.metaDescription = SeoResources.HomePageMeta;

            var university = HttpContext.Items[UniversityCookie.CookieName] as UniversityCookie;
            //var university = m_CookieHelper.ReadCookie<UniversityCookie>(UniversityCookie.CookieName);

            if (university != null && string.IsNullOrEmpty(universityName))
            {

                return RedirectToRoute("UniversityLink", new { invId, universityName = university.UniversityName, step });
            }
            if (university != null && FlashcardUniversities.Contains(university.UniversityId))
            {
                var flashcardPromo = m_CookieHelper.ReadCookie<UniversityFlashcardPromo>(UniversityFlashcardPromo.CookieName);
                if (flashcardPromo == null)
                {
                    return RedirectToRoute("Promotion");
                }
                ViewBag.promoEnable = true;


            }
            if (!string.IsNullOrEmpty(universityName) && university == null)
            {
                return RedirectToRoute("homePage", new { invId });
            }

            if (!string.IsNullOrEmpty(invId))
            {
                var guid = GuidEncoder.TryParseNullableGuid(invId);
                if (guid.HasValue)
                {
                    m_CookieHelper.InjectCookie(Invite.CookieName, new Invite { InviteId = guid.Value });
                }
            }
            var query = new GetHomePageQuery(university?.UniversityId);
            var homeStats = await ZboxReadService.GetHomePageDataAsync(query);

            return View("Index", homeStats);
        }


        //TODO: add cache on this
        [ActionName("Boxes"), HttpGet]
        public async Task<JsonResult> BoxesAsync()
        {
            var value = m_CookieHelper.ReadCookie<UniversityCookie>(UniversityCookie.CookieName);
            long? universityId = null;
            if (value != null)
            {
                universityId = value.UniversityId;
            }
            var country = "US";
            var prefix = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (prefix.ToLower() == "he")
            {
                country = "IL";
            }
            var result =
                await ZboxReadService.GetUniversityBoxesAsync(new GetHomeBoxesUniversityQuery(universityId, country));
            return JsonOk(result);

        }


        [Route("account/signin")]
        public ActionResult SignIn(string lang, string invId)
        {
            return RedirectToRoutePermanent("homePage", new { step = "signin", lang, invId });
        }
        [Route("account/signup")]
        public ActionResult SignUp(string lang, string invId)
        {
            return RedirectToRoutePermanent("homePage", new { step = "signup", lang, invId });
        }




        //don't put in here route attribute
        [DonutOutputCache(CacheProfile = "FullPage")]
        [NoUniversity]
        public ActionResult IndexEmpty()
        {
            return View("Empty");
        }

        [DonutOutputCache(CacheProfile = "FullPage")]
        [Route("terms", Name = "TOS")]
        public ActionResult Terms()
        {
            ViewBag.title = SeoResources.TermsTitle;
            ViewBag.metaDescription = SeoResources.TermsMeta;
            return View("TermsOfService");
        }


        [DonutOutputCache(CacheProfile = "FullPage")]
        [Route("help", Name = "Help")]
        public async Task<ViewResult> HelpAsync()
        {
            ViewBag.title = SeoResources.HelpTitle;
            ViewBag.metaDescription = SeoResources.HelpMeta;

            using (var stream = await m_BlobProvider.Value.GetFaqQuestionAsync())
            {
                var data = XDocument.Load(stream);
                var model = from category in data.Descendants("category")
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
                                        Answer = s.Element("answer")?.Value,
                                        Question = s.Element("question")?.Value,
                                        Order = int.Parse(s.Attribute("order").Value)

                                    }).OrderBy(s => s.Order).ToList()

                            };
                return View("Help2", model.Where(w => string.Equals(w.Language,
                     Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, StringComparison.CurrentCultureIgnoreCase)));
            }
        }
        [DonutOutputCache(CacheProfile = "FullPage")]
        [Route("jobs", Name = "Jobs")]
        public async Task<ActionResult> JobsAsync()
        {
            ViewBag.title = SeoResources.JobsTitle;
            ViewBag.metaDescription = SeoResources.JobsMeta;

            using (var stream = await m_BlobProvider.Value.GetJobsXmlAsync())
            {
                var data = XDocument.Load(stream);
                var model = from category in data.Descendants("category")
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
                                        Answer = s.Element("answer")?.Value,
                                        Question = s.Element("question")?.Value,
                                        Order = int.Parse(s.Attribute("order").Value)

                                    }).OrderBy(s => s.Order).ToList()
                            };
                return View("help2", model);
            }
        }
        [DonutOutputCache(CacheProfile = "FullPage")]
        [Route("privacy", Name = "Privacy")]
        public ActionResult Privacy()
        {
            ViewBag.title = SeoResources.PrivacyTitle;
            ViewBag.metaDescription = SeoResources.PrivacyMeta;
            return View();
        }

        [DonutOutputCache(CacheProfile = "FullPage")]
        [Route("aboutus", Name = "AboutUs")]
        public ActionResult AboutUs()
        {
            ViewBag.title = SeoResources.AboutUsTitle;
            ViewBag.metaDescription = SeoResources.AboutUsMeta;
            return View();
        }

        [Route("home")]
        public ActionResult HomeIndex()
        {
            return RedirectToRoutePermanent("homePage");
        }

        //remove output cache due to language issues

        //  [DonutOutputCache(CacheProfile = "FullPage")]
        public ViewResult Blog(string lang)
        {

            if (!string.IsNullOrEmpty(lang))
            {
                LanguageMiddleware.ChangeThreadLanguage(lang);
            }
            var iFrameSrc = "https://spitballblog.wordpress.com/";
            if (Thread.CurrentThread.CurrentUICulture.Name.ToLower().StartsWith("he"))
            {
                iFrameSrc = "https://spitballcoh.wordpress.com/";
            }
            ViewBag.title = SeoResources.BlogTitle;
            ViewBag.metaDescription = SeoResources.BlogMeta;
            ViewBag.iFrameSrc = iFrameSrc;
            return View();
        }

        [Route("product", Name = "Product")]
        [Route("product/{lang:regex(^(en|he))}", Name = "Product2")]
        public ActionResult Product(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                if (lang.ToLower() == "en")
                {
                    return RedirectToRoutePermanent("Product");
                }
                LanguageMiddleware.ChangeThreadLanguage(lang);
            }

            ViewBag.title = SeoResources.ProductTitle;
            ViewBag.metaDescription = SeoResources.ProductMeta;
            return View();
        }

        [DonutOutputCache(CacheProfile = "FullPage")]
        [Route("features", Name = "Features")]
        [Route("features/{lang:regex(^(en|he))}", Name = "Features2")]
        public ActionResult Features(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                if (lang.ToLower() == "en")
                {
                    return RedirectToRoutePermanent("Features");
                }
                LanguageMiddleware.ChangeThreadLanguage(lang);
            }
            ViewBag.title = SeoResources.FeaturesTitle;
            ViewBag.metaDescription = SeoResources.HelpMeta;
            return View();
        }

        [Route("home/privacy")]
        public ActionResult PrivacyOld()
        {
            return RedirectToRoutePermanent("Privacy");
        }
        [Route("home/aboutus")]
        public ActionResult AboutUsOld()
        {
            return RedirectToRoutePermanent("AboutUs");
        }


        [Route("advertiseWithUs", Name = "Advertise")]
        [DonutOutputCache(CacheProfile = "FullPage")]
        public ViewResult AdvertiseWithUs()
        {
            ViewBag.title = SeoResources.AdvertiseWithUsTitle;
            ViewBag.metaDescription = SeoResources.AdvertiseWithUsMeta;
            return View();

        }

        [Route("apps", Name = "apps")]
        [DonutOutputCache(CacheProfile = "FullPage")]
        public ActionResult Apps()
        {
            ViewBag.title = SeoResources.AppsTitle;
            ViewBag.metaDescription = SeoResources.AppsMeta;
            return View();
        }

        [Route("promotion", Name = "Promotion")]
        [DonutOutputCache(CacheProfile = "FullPage")]
        public async Task<ActionResult> PromotionAsync()
        {
            var value = m_CookieHelper.ReadCookie<UniversityCookie>(UniversityCookie.CookieName);
            if (value == null)
            {
                return RedirectToRoute("homePage");
            }
            if (!FlashcardUniversities.Contains(value.UniversityId))
            {
                return RedirectToRoute("homePage");
            }
            m_CookieHelper.InjectCookie(UniversityFlashcardPromo.CookieName,
                new UniversityFlashcardPromo());
            var query = new GetHomePageQuery(value.UniversityId);
            var homeStats = await ZboxReadService.GetHomePageDataAsync(query);
            ViewBag.promoEnable = true;
            //homeStats.FlashcardPromo = true;
            return View("Promotion", homeStats);
        }

        [Route("classnotes", Name = "classnotes")]
        [Route("classnotes/{lang:regex(^(en|he))}", Name = "classnotes2")]
        public ActionResult ClassNotes(string lang)
        {

            if (!string.IsNullOrEmpty(lang))
            {
                if (lang.ToLower() == "en")
                {
                    return RedirectToRoutePermanent("classnotes");
                }
                LanguageMiddleware.ChangeThreadLanguage(lang);
            }
            ViewBag.title = SeoResources.ClassNotesTitle;
            ViewBag.metaDescription = SeoResources.ClassNotesMeta;
            //var items = await ZboxReadService.GetItemsPageDataAsync();

            return View();
        }

        [Route("courses", Name = "courses")]
        [Route("courses/{lang:regex(^(en|he))}", Name = "courses2")]
        public async Task<ActionResult> CoursesAsync(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                if (lang.ToLower() == "en")
                {
                    return RedirectToRoutePermanent("courses");
                }
                LanguageMiddleware.ChangeThreadLanguage(lang);
            }
            ViewBag.title = SeoResources.CoursesTitle;
            ViewBag.metaDescription = SeoResources.CoursesMeta;
            var courses = await ZboxReadService.GetCoursesPageDataAsync();
            return View("Courses", courses);
        }
        [Route("home/help")]
        public ActionResult HelpOld()
        {
            return RedirectToRoutePermanent("Help");
        }



        [DonutOutputCache(CacheProfile = "FullPage")]
        // ReSharper disable once InconsistentNaming
        public ActionResult IFrame()
        {
            return View();
        }



        [ChildActionOnly]
        public ActionResult AntiForgeryToken()
        {
            return PartialView("_AntiForgeryToken");
        }








        [AllowAnonymous]
        [HttpGet]
        [NoAsyncTimeout, ActionName("SiteMap")]
        [OutputCache(Duration = 2 * TimeConst.Day, VaryByParam = "index", Location = OutputCacheLocation.Any)]
        public async Task<ActionResult> SiteMapAsync(int? index)
        {
            if (!index.HasValue)
            {
                var contentIndex = await GetSitemapIndexAsync();
                return Content(contentIndex, "application/xml", Encoding.UTF8);
            }
            var content = await GetSitemapXmlAsync(index.Value);
            return Content(content, "application/xml", Encoding.UTF8);
        }

        private async Task<string> GetSitemapIndexAsync()
        {
            const string sitemapsNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace xmlns = sitemapsNamespace;
            var noOfSiteMaps = await ZboxReadService.GetSeoItemCountAsync();

            // ReSharper disable once StringLiteralTypo
            var root = new XElement(xmlns + "sitemapindex");
            for (int i = 1; i <= noOfSiteMaps; i++)
            {
                root.Add(
                    new XElement(xmlns + "sitemap",
                        new XElement(xmlns + "loc", $"https://www.spitball.co/sitemap-{i}.xml")
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
        private async Task<string> GetSitemapXmlAsync(int index)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace xhtml = "http://www.w3.org/1999/xhtml";
            var nodes = await GetSitemapNodesAsync(index);

            var root = new XElement(xmlns + "urlset",
                //new XAttribute("xmlns", xmlns.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "xhtml", xhtml));



            foreach (var node in nodes)
            {
                var locContent = new XElement(xmlns + "loc", node.Url);
                var priorityContent = node.Priority == null
                        ? null
                        : new XElement(xmlns + "priority",
                            node.Priority.Value.ToString("F1", CultureInfo.InvariantCulture));
                var lastmodContent = node.LastModified == null
                    ? null
                    : new XElement(xmlns + "lastmod",
                        // ReSharper disable once StringLiteralTypo
                        node.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"));
                var frequencyContent = node.Frequency == null
                    ? null
                    : new XElement(xmlns + "changefreq", node.Frequency.Value.ToString().ToLowerInvariant());


                var url = new XElement(xmlns + "url", locContent, priorityContent, lastmodContent, frequencyContent);
                //var langLink = new List<XElement>();
                if (node.SitemapLangNodes != null)
                {
                    foreach (var lang in node.SitemapLangNodes)
                    {
                        var langNode = new XElement(xhtml + "link",
                        new XAttribute("rel", "alternate"),
                        new XAttribute("hreflang", lang.Language),
                        new XAttribute("href", lang.Url));

                        url.Add(langNode);
                        //        langLink.Add(langNode);
                    }
                }


                root.Add(url);

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
        private async Task<IEnumerable<SitemapNode>> GetSitemapNodesAsync(int index)
        {
            var requestContext = ControllerContext.RequestContext;
            var nodes = new List<SitemapNode>();
            if (index == 1)
            {
                nodes.Add(
                new SitemapNode(requestContext, "homePage", null)
                {
                    Priority = 1.0,
                    Frequency = SitemapFrequency.Daily
                });
                nodes.Add(
                     new SitemapNode(requestContext, "Blog", new { lang = "en-us" })
                     {
                         Priority = 0.95,
                         Frequency = SitemapFrequency.Daily
                     });
                //Something is wrong with this url
                //nodes.Add(
                //    new SitemapNode(requestContext, "Blog", new { lang = "he-il" })
                //    {
                //        Priority = 0.95,
                //        Frequency = SitemapFrequency.Daily
                //    });
                nodes.Add(
                    new SitemapNode(requestContext, "AboutUs", null)
                    {
                        Priority = 0.95,
                        Frequency = SitemapFrequency.Daily
                    });
                nodes.Add(
                   new SitemapNode(requestContext, "Help", null)
                   {
                       Priority = 0.95,
                       Frequency = SitemapFrequency.Daily
                   });
                //nodes.Add(
                //  new SitemapNode(requestContext, "homePage", new { step = "signin" })
                //  {
                //      Priority = 0.95,
                //      Frequency = SitemapFrequency.Daily
                //  });
                //nodes.Add(
                //  new SitemapNode(requestContext, "homePage", new { step = "signup" })
                //  {
                //      Priority = 0.95,
                //      Frequency = SitemapFrequency.Daily
                //  });
                nodes.Add(
                   new SitemapNode(requestContext, "Jobs", null)
                   {
                       Priority = 0.95,
                       Frequency = SitemapFrequency.Daily
                   });

                nodes.Add(
                    new SitemapNode(requestContext, "Privacy", null)
                    {
                        Priority = 0.8,
                        Frequency = SitemapFrequency.Daily
                    });
                nodes.Add(
                    new SitemapNode(requestContext, "TOS", null)
                    {
                        Priority = 0.8,
                        Frequency = SitemapFrequency.Daily
                    });
                nodes.Add(
                    new SitemapNode(requestContext, "Advertise", null)
                    {
                        Priority = 0.8,
                        Frequency = SitemapFrequency.Daily
                    });
                nodes.AddRange(SitemapNode.SiteMapNodesWithLang(requestContext,
                    new SitemapNodeLangHelper("Product", null, "en"),
                    new SitemapNodeLangHelper("Product2", new { lang = "he" }, "he")
                    ));
                nodes.AddRange(SitemapNode.SiteMapNodesWithLang(requestContext,
                   new SitemapNodeLangHelper("Features", null, "en"),
                   new SitemapNodeLangHelper("Features2", new { lang = "he" }, "he")
                   ));
                nodes.AddRange(SitemapNode.SiteMapNodesWithLang(requestContext,
                   new SitemapNodeLangHelper("classnotes", null, "en"),
                   new SitemapNodeLangHelper("classnotes2", new { lang = "he" }, "he")
                   ));
                nodes.AddRange(SitemapNode.SiteMapNodesWithLang(requestContext,
                   new SitemapNodeLangHelper("courses", null, "en"),
                   new SitemapNodeLangHelper("courses2", new { lang = "he" }, "he")
                   ));


                nodes.Add(
                   new SitemapNode(requestContext, "apps", null)
                   {
                       Priority = 0.8,
                       Frequency = SitemapFrequency.Daily
                   });
            }

            var seoItems = await ZboxReadService.GetSeoItemsAsync(index);
            nodes.AddRange(seoItems.Where(w => !string.IsNullOrEmpty(w.Url))
                .Select(s => new SitemapNode(s.Url, requestContext)));

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
        public JsonResult Version()
        {
            return JsonOk(VersionHelper.CurrentVersion(true));
        }

    }
}
