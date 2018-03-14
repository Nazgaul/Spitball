using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Domain;
using Cloudents.Infrastructure.Search;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Converters
{
    [UsedImplicitly]
    public class BingConverter : ITypeConverter<BingSearch.WebPage, SearchResult>
    {
        private readonly IKeyGenerator _keyGenerator;
        private readonly IReplaceImageProvider _imageProvider;
        private readonly DomainParser _domainParser;

        public BingConverter(IKeyGenerator keyGenerator, IReplaceImageProvider imageProvider, DomainParser domainParser)
        {
            _keyGenerator = keyGenerator;
            _imageProvider = imageProvider;
            _domainParser = domainParser;
        }

        public SearchResult Convert(BingSearch.WebPage source, SearchResult destination, ResolutionContext context)
        {
            var url = new Uri(source.Url);
            var highlight = Enumerable.Empty<string>();
            if (context.Items.TryGetValue("query", out var p) && p is IEnumerable<string> z)
            {
                highlight = z;
            }

            if (Uri.TryCreate(source.OpenGraphImage?.ContentUrl, UriKind.Absolute, out var image))
            {
                image = image.ChangeToHttps();
            }

            var domain = _domainParser.GetDomain(url.Host);

            image = _imageProvider.ChangeImageIfNeeded(domain, image);
            var result = new SearchResult
            {
                Url = source.Url,
                Id = _keyGenerator.GenerateKey(source.Url),
                Image = image,
                Snippet = source.Snippet.HighlightKeyWords(highlight, false),
                Source = domain,
                Title = source.Name,
            };

            if (string.Equals(domain, "courseHero", StringComparison.OrdinalIgnoreCase))
            {
                result.Url =
                    $"http://shareasale.com/r.cfm?b=661825&u=1469379&m=55976&urllink={url.Host + url.PathAndQuery + url.Fragment}&afftrack=";
            }
            return result;
        }
    }
}