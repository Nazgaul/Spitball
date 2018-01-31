using System;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Search;

namespace Cloudents.Infrastructure.Converters
{
    public class BingConverter : ITypeConverter<BingSearch.WebPage, SearchResult>
    {
        private readonly IKeyGenerator _keyGenerator;

        public BingConverter(IKeyGenerator keyGenerator)
        {
            _keyGenerator = keyGenerator;
        }

        public SearchResult Convert(BingSearch.WebPage source, SearchResult destination, ResolutionContext context)
        {
            if (Uri.TryCreate(source.OpenGraphImage?.ContentUrl, UriKind.Absolute, out var image))
            {
                image = image.ChangeToHttps();
            }
            var url = new Uri(source.Url);
            var result = new SearchResult
            {
                Url = source.Url,
                Id = _keyGenerator.GenerateKey(source.Url),
                Image = image,
                Snippet = source.Snippet,
                Source = url.Host,
                Title = source.Name,
            };
            if (string.Equals(url.Host, "www.courseHero.com", StringComparison.InvariantCultureIgnoreCase))
            {
                result.Url =
                    $"http://shareasale.com/r.cfm?b=661825&u=1469379&m=55976&urllink={url.Host + url.PathAndQuery + url.Fragment}&afftrack=";
            }

            return result;
        }
    }
}