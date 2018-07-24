using System;
using System.Text;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Services
{
    [UsedImplicitly]
    public class UrlConst : IUrlBuilder
    {
        private readonly IUrlHelper _urlHelper;

        public UrlConst(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
            //_systemUrl = configuration.FunctionEndpoint + "/api/redirect";
        }

        //private readonly string _systemUrl;// = "https://functions.spitball.co/api/redirect";

        public string BuildRedirectUrl(string url, string host, int? location)
        {
            if (host.Contains("spitball", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            if (url.Contains("spitball", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }
            if (Uri.TryCreate(url,UriKind.RelativeOrAbsolute,out var p))
            {
                if (!p.IsAbsoluteUri)
                {
                    return url;
                }
            }
            //var nvc = new NameValueCollection
            //{
            //    ["url"] = url,
            //    ["host"] = host,

            //};
            //if (location.HasValue)
            //{
            //    nvc["location"] = location.ToString();
            //}

            var uri = _urlHelper.Action("index", "url", new {url, host, location});
            return uri;
        }

        public static string NameToQueryString(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }
            var previousChar = '\0';
            var sb = new StringBuilder();
            // name = name.Replace(Convert.ToChar(160), ' ');
            //name = name.Replace("\n", string.Empty);

            foreach (var character in name)
            {
                if (!char.IsLetterOrDigit(character) && !char.IsWhiteSpace(character) && !char.IsPunctuation(character))
                    continue;
                switch (character)
                {
                    case (char)160:
                    case '\n':
                    case '<':
                    case '>':
                    case '*':
                    case '%':
                    case '&':
                    case ':':
                    case '\\':
                    case '/':
                    case ';':
                    case '?':
                    case '@':
                    case '=':
                    case '+':
                    case '$':
                    case ',':
                    case '{':
                    case '}':
                    case '|':
                    case '^':
                    case '[':
                    case ']':
                    case '`':
                    case '"':
                    case '#':
                    case '\'':
                    case '(':
                    case ')':
                        continue;
                    case ' ':
                    case '_':
                    case '-':
                    case (char)65288:
                    case (char)65289:
                        if (previousChar != '-')
                        {
                            sb.Append('-');
                        }
                        previousChar = '-';

                        break;
                    default:
                        previousChar = character;
                        sb.Append(character);
                        break;
                }
            }
            return sb.ToString().ToLowerInvariant();
        }
    }
}
