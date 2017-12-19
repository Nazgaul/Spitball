using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace Cloudents.Core.Extension
{
    public static class UriBuilderExtension
    {
        public static void AddQuery(this UriBuilder builder, NameValueCollection val)
        {
            if (val == null)
            {
                return;
            }
            var query = string.Join("&", val.AllKeys.Select(key => $"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(val[key])}"));
            if (builder.Query != null && builder.Query.Length > 1)
            {
                builder.Query = builder.Query.Substring(1) + "&" + query;
            }
            else
            {
                builder.Query = query;
            }
        }
    }

    
}
