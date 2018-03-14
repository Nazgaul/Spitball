using JetBrains.Annotations;
using Nager.PublicSuffix;

namespace Cloudents.Infrastructure.Domain
{
    [UsedImplicitly]
    public class DomainParser : IDomainParser
    {
        private readonly Nager.PublicSuffix.DomainParser _parser;
        public DomainParser(ICacheProvider cache)
        {
            var provider = new WebTldRuleProvider(cacheProvider: cache);
            _parser = new Nager.PublicSuffix.DomainParser(provider);
        }

        [CanBeNull]
        public string GetDomain(string host)
        {
            var domainName = _parser.Get(host);
            return domainName?.Domain;
        }
    }

    public interface IDomainParser
    {
        [CanBeNull]
        string GetDomain(string host);
    }
}