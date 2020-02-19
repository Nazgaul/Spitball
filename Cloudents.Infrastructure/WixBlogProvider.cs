using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure
{
    public class WixBlogProvider : IBlogProvider
    {
        public Task<IEnumerable<DashboardBlogDto>> GetBlogAsync(Country country, CancellationToken token)
        {
            var blogName = "English";
            if (country == Country.Israel)
            {
                blogName = "Hebrew";
            }

            using var reader = XmlReader.Create("https://www.blog.spitball.co/blog-feed.xml");
            var formatter = new Rss20FeedFormatter();
            formatter.ReadFrom(reader);
            var items = formatter.Feed.Items;
            token.ThrowIfCancellationRequested();

            var result = items
                .Where(w => w.Categories.Any(a => a.Name == "Teachers") && w.Categories.Any(a => a.Name == blogName))
                .Select(s => new DashboardBlogDto
                {
                    Image = s.Links.Where(w => w.RelationshipType == "enclosure").Select(s2 => s2.Uri).First()
                        .ChangeToHttps(),
                    Url = s.Links.Where(w => w.RelationshipType == "alternate").Select(s2 => s2.Uri).First(),
                    Title = s.Title.Text,
                    Uploader = s.ElementExtensions
                        .ReadElementExtensions<string>("creator", "http://purl.org/dc/elements/1.1/").FirstOrDefault(),
                    Create = s.PublishDate
                }).OrderByDescending(o => o.Create).Take(3);
            return Task.FromResult(result);


        }
    }
}