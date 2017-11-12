using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Cloudents.Web.Controllers
{
    public class SiteMapController : Controller
    {
        private readonly IReadRepositorySingle<IEnumerable<SiteMapDto>> _readRepository;

        public SiteMapController(IReadRepositorySingle<IEnumerable<SiteMapDto>> readRepository)
        {
            _readRepository = readRepository;
        }

        [Route("sitemap.xml")]
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var content = await GetSitemapIndexAsync(token).ConfigureAwait(false);
            return Content(content, "application/xml");
        }


        private async Task<string> GetSitemapIndexAsync(CancellationToken token)
        {
            
            XNamespace nameSpace = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var model = await _readRepository.GetAsync(token).ConfigureAwait(false);

            const int pageSize = 49950;
            // ReSharper disable once StringLiteralTypo
            var root = new XElement(nameSpace + "sitemapindex");
            root.Add(
                new XElement(nameSpace + "sitemap",
                    new XElement(nameSpace + "loc", $"https://www.spitball.co/sitemap-{SeoType.Static}-0.xml")
                )
            );

            foreach (var elem in model)
            {
                for (var i = 0; i <= elem.Count / pageSize; i++)
                {
                    root.Add(
                        new XElement(nameSpace + "sitemap",
                            new XElement(nameSpace + "loc", $"https://www.spitball.co/sitemap-{elem.Type}-{i}.xml")
                        )
                    );
                }
            }
            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            return document.ToString();
        }
    }
}