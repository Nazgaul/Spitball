﻿using System;
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
            builder.Query = string.Join("&", val.AllKeys.Select(key =>
                $"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(val[key])}"));
        }
    }
}
