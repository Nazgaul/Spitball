using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core
{
    public class UrlRedirectBuilder : IUrlRedirectBuilder
    {
        private readonly IUrlBuilder _urlBuilder;

        public UrlRedirectBuilder(IUrlBuilder urlBuilder)
        {
            _urlBuilder = urlBuilder;
        }

        public IEnumerable<T> BuildUrl<T>(IEnumerable<T> result, int page = 0, int sizeOfPage = 0) where T : IUrlRedirect
        {
            return result.Select((s, i) =>
            {
                s.Url = _urlBuilder.BuildRedirectUrl(s.Url, s.Source, (page * sizeOfPage) + i);
                return s;
            });
        }
    }
}