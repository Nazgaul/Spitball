using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;

namespace Cloudents.Infrastructure
{
    public class WixBlogProvider : IBlogProvider
    {
        private readonly ICacheProvider _cacheProvider;

        public WixBlogProvider(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        [Cache(TimeConst.Hour, "Blog", false)]
        public Task<IEnumerable<DashboardBlogDto>?> GetBlogAsync(BlogQuery query,
            CancellationToken token)
        {
            var cacheKey = $"{query.GetType().Name}_{query.AsDictionary(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToContentString()}";

            IEnumerable<DashboardBlogDto>? result = _cacheProvider.Get<IEnumerable<DashboardBlogDto>>(cacheKey, "Blog");
            if (result == null)
            {
                using var reader = XmlReader.Create("https://www.blog.spitball.co/blog-feed.xml");
                var formatter = new Rss20FeedFormatter();
                formatter.ReadFrom(reader);
                var items = formatter.Feed.Items;
                token.ThrowIfCancellationRequested();

                result = (items
                     .Where(w => w.Categories.Any(a => a.Name == query.Category)
                                 && w.Categories.Any(a => a.Name == query.BlogName))
                     .Select(s => new DashboardBlogDto
                     {
                         Image = s.Links.Where(w => w.RelationshipType == "enclosure")
                             .Select(s2 => s2.Uri).FirstOrDefault()?.ChangeToHttps(),
                         Url = s.Links.Where(w => w.RelationshipType == "alternate")
                             .Select(s2 => s2.Uri).FirstOrDefault(),
                         Title = s.Title.Text,
                         Uploader = s.ElementExtensions
                             .ReadElementExtensions<string>("creator", "http://purl.org/dc/elements/1.1/")
                             .FirstOrDefault(),
                         Create = s.PublishDate
                     }).OrderByDescending(o => o.Create).Take(query.Amount));

                result = result.Where(w => w.Image != null).ToList();

                _cacheProvider.Set<IEnumerable<DashboardBlogDto>>(cacheKey, "Blog", result, TimeSpan.FromHours(1),
                    false);
                return Task.FromResult<IEnumerable<DashboardBlogDto>?>(result);
            }

            return Task.FromResult(result);
        }
    }
}