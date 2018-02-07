using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core
{
    public class UrlRedirectBuilder<T> : IUrlRedirectBuilder<T> where T : IUrlRedirect
    {
        private readonly IUrlBuilder _urlBuilder;

        public UrlRedirectBuilder(IUrlBuilder urlBuilder)
        {
            _urlBuilder = urlBuilder;
        }

        public IEnumerable<T> BuildUrl(int page, int sizeOfPage, IEnumerable<T> result)
        {
            return result.Select((s, i) =>
            {
                s.Url = _urlBuilder.BuildRedirectUrl(s.Url, s.Source, (page * sizeOfPage) + i);
                return s;
            });
        }

        public IEnumerable<T> BuildUrl(IEnumerable<T> result)
        {
            return result.Select(s =>
            {
                s.Url = _urlBuilder.BuildRedirectUrl(s.Url, s.Source, null);
                return s;
            });
        }
    }

    public interface IUrlRedirect
    {
        string Url { get; set; }
        string Source { get; }
    }
}