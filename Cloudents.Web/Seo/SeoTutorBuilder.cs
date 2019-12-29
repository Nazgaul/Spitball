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

        public IEnumerable<SitemapNode> GetUrls(int index)
        {
            var t = _session.Query<Tutor>()
                .Fetch(f => f.User)
                .Where(w => (!w.User.LockoutEnd.HasValue || DateTime.UtcNow >= w.User.LockoutEnd.Value))
                .Where(w => w.State == ItemState.Ok && w.User.Country != Country.India.Name)
                .Take(SiteMapController.PageSize)
                .Skip(SiteMapController.PageSize * index)
                .Select(s => new { s.Id, s.User.Name,s.User.ImageName });

            foreach (var item in t)
            {



                var url =  _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Tutor, new
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
                        new SitemapImage(_urlBuilder.BuildUserImageEndpoint(item.Id,item.ImageName,item.Name))
                        {
                            Caption = $"{item.Name} profile image"
                        }
                    }
                };

            }
        }
    }
}