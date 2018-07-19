using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Domain;
using Cloudents.Infrastructure.Search;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Converters
{
    [UsedImplicitly]
    public class BingConverter : ITypeConverter<BingWebPage, SearchResult>
    {
        private readonly IKeyGenerator _keyGenerator;
        private readonly IReplaceImageProvider _imageProvider;
        private readonly IDomainParser _domainParser;

        public const string KeyTermHighlight = "query";
        public const string KeyPriority = "priority";

        public BingConverter(IKeyGenerator keyGenerator, IReplaceImageProvider imageProvider, IDomainParser domainParser)
        {
            _keyGenerator = keyGenerator;
            _imageProvider = imageProvider;
            _domainParser = domainParser;
        }

        public SearchResult Convert(BingWebPage source, SearchResult destination, ResolutionContext context)
        {
            var url = new Uri(source.Url);
            var highlight = Enumerable.Empty<string>();
            if (context.Items.TryGetValue(KeyTermHighlight, out var p) && p is IEnumerable<string> z)
            {
                highlight = z;
            }
            var domain = _domainParser.GetDomain(url.Host);
            var priority = PrioritySource.Unknown;
            if (Uri.TryCreate(source.OpenGraphImage?.ContentUrl, UriKind.Absolute, out var image))
            {
                image = image.ChangeToHttps();
            }
            if (context.Items.TryGetValue(KeyPriority, out var val) 
                && val is IReadOnlyDictionary<string, PrioritySource> priorities 
                && domain != null)
            {
                if (domain.Equals("spitball", StringComparison.OrdinalIgnoreCase))
                {
                    domain = PrioritySource.CloudentsSource;
                }

                if (priorities.TryGetValue(domain, out var priorityTemp))
                {
                    priority = priorityTemp;
                    domain = priority.Source;
                }
            }

            var result = new SearchResult
            {
                Url = source.Url,
                Id = _keyGenerator.GenerateKey(source.Url),
                Image = _imageProvider.ChangeImageIfNeeded(domain, image),
                Snippet = source.Snippet.HighlightKeyWords(highlight, false),
                Source = domain,
                Title = source.Name,
                PrioritySource = priority,
            };

            
            return result;
        }
    }
}