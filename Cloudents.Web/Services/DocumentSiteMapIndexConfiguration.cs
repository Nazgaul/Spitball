using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using SimpleMvcSitemap;
using SimpleMvcSitemap.StyleSheets;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Web.Extensions;

namespace Cloudents.Web.Services
{
    public class DocumentSiteMapIndexConfiguration : ISitemapIndexConfiguration<DocumentSeoDto>
    {
        private readonly IUrlHelper _urlHelper;
        public DocumentSiteMapIndexConfiguration(int? currentPage, IStatelessSession statelessSession,
            IUrlHelper urlHelper)
        {
            CurrentPage = currentPage;
            _urlHelper = urlHelper;
            DataSource = statelessSession.Query<Document>()
                .Where(w => w.State == ItemState.Ok)
                .Fetch(f => f.University)
                .Select(s => new DocumentSeoDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Country = s.University.Country,
                    CourseName = s.Course.Name,
                    UniversityName = s.University.Name
                });
        }

        public SitemapIndexNode CreateSitemapIndexNode(int currentPage)
        {

            return new SitemapIndexNode(_urlHelper.RouteUrl("ProductSitemap", new { currentPage }));
        }

        public SitemapNode CreateNode(DocumentSeoDto source)
        {
            return new SitemapNode(_urlHelper.RouteUrl(SeoTypeString.Document, new
            {
                universityName = FriendlyUrlHelper.GetFriendlyTitle(source.UniversityName),
                courseName = FriendlyUrlHelper.GetFriendlyTitle(source.CourseName),
                source.Id,
                name = FriendlyUrlHelper.GetFriendlyTitle(source.Name)
            }));
        }

        public IQueryable<DocumentSeoDto> DataSource { get; }
        public int Size => 50000;
        public int? CurrentPage { get; }
        public List<XmlStyleSheet> SitemapStyleSheets { get; protected set; }
        public List<XmlStyleSheet> SitemapIndexStyleSheets { get; protected set; }
        public bool UseReverseOrderingForSitemapIndexNodes => false;
    }
}
