using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nager.PublicSuffix;

namespace Cloudents.Infrastructure
{
    public class DomainParser
    {
        private readonly Nager.PublicSuffix.DomainParser _parser;
        public DomainParser()
        {
            var provider = new WebTldRuleProvider();
            _parser = new Nager.PublicSuffix.DomainParser(provider);
        }


        [CanBeNull]
        public string GetDomain(string host)
        {
            var domainName = _parser.Get(host);
            return domainName?.Domain;
        }
    }

}