using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cloudents.Core.Extension
{
    public static class UriBuilderExtension
    {
        public static void AddQuery(this UriBuilder builder, NameValueCollection val)
        {
            builder.Query = string.Join("&", val.AllKeys.Select(key =>
                $"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(val[key])}"));
        }
    }
}
