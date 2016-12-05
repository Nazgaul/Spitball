using System;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetCountryByIpQuery : IQueryCache
    {
        public GetCountryByIpQuery(long ipAddress)
        {
            IpAddress = ipAddress;
        }

        public long IpAddress { get; }
        public string CacheKey => IpAddress.ToString(CultureInfo.InvariantCulture);

        public string CacheRegion => "IpAddress";

        public TimeSpan Expiration => TimeSpan.FromDays(365);
    }
}
