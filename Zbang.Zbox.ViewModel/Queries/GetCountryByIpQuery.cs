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

        public long IpAddress { get; private set; }
        public string CacheKey
        {
            get { return IpAddress.ToString(CultureInfo.InvariantCulture); }
        }

        public string CacheRegion
        {
            get { return "IpAddress"; }
        }

        public TimeSpan Expiration
        {
            get { return TimeSpan.FromDays(365); }
        }
    }
}
