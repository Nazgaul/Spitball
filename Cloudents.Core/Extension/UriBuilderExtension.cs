using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace Cloudents.Core.Extension
{
    public static class UriBuilderExtensions
    {
        public static UriBuilder AddQuery(this UriBuilder builder, NameValueCollection? nvc)
        {
            if (nvc == null)
            {
                return builder;
            }
            //var query = string.Join("&", val.AllKeys.Select(key => 
            //   $"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(val[key])}"));

            var query = string.Join("&", nvc.AllKeys.SelectMany(key => (nvc.GetValues(key) ?? Enumerable.Empty<string>()).Select(val => string.Concat(key, "=", WebUtility.UrlEncode(val)))));
            if (builder.Query.Length > 1)
            {
                builder.Query = builder.Query.Substring(1) + "&" + query;
            }
            else
            {
                builder.Query = query;
            }

            return builder;
        }

        public static UriBuilder AddQuery(this UriBuilder builder, object? obj)
        {
            if (obj == null)
            {
                return builder;
            }

            NameValueCollection formFields = new NameValueCollection();

            obj.GetType().GetProperties()
                .ToList()
                .ForEach(pi =>
                {
                    var val = pi.GetValue(obj, null);
                    if (val != null)
                    {
                        formFields.Add(pi.Name, val.ToString());
                    }
                });

            return builder.AddQuery(formFields);
        }
    }
}
