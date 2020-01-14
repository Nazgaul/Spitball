using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Controllers;
using Cloudents.Web.Seo.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Web.Seo
{

    public class SeoTutorBuilder : IBuildSeo
    {
        private readonly IStatelessSession _session;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlBuilder _urlBuilder;

        public SeoTutorBuilder(IStatelessSession session, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor, IUrlBuilder urlBuilder)
        {
            _session = session;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
            _urlBuilder = urlBuilder;
        }

        public IEnumerable<SitemapNode> GetUrls(bool isFrymo, int index)
        {
            var t = _session.Query<ReadTutor>();
            if (isFrymo)
            {
                t = t.Where(w => w.Country == Country.India.Name);
            }
            else
            {
                t = t.Where(w => w.Country != Country.India.Name);
            }


            var tutors = t.Take(SiteMapController.PageSize)
              .Skip(SiteMapController.PageSize * index)
              .Select(s => new { s.Id, s.Name, s.ImageName });

            foreach (var item in tutors)
            {
                var url = _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Tutor, new
                {
                    item.Id,
                    item.Name
                });

                yield return new SitemapNode(url)
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Priority = 1,
                    TimeStamp = DateTime.UtcNow,
                    Images = new List<SitemapImage>()
                    {
                        new SitemapImage(_urlBuilder.BuildUserImageEndpoint(item.Id,item.ImageName,item.Name, new
                        {
                            width= "214",
                            height = "240"
                        }))
                        {
                            Caption = $"{item.Name} profile image"
                        }
                    }
                };

            }
        }

        private static bool FilterCountry(ReadTutor w, bool isFrymo)
        {
            if (isFrymo)
            {
                return w.Country == Country.India.Name;
            }

            return w.Country != Country.India.Name;
        }
    }
}