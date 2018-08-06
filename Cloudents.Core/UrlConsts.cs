﻿using System;
using System.Collections.Specialized;
using System.Text;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core
{
    [UsedImplicitly]
    public class UrlConst : IUrlBuilder
    {
        private readonly string _webSiteEndPoint;

        public UrlConst(IConfigurationKeys configuration)
        {
            _webSiteEndPoint = configuration.SiteEndPoint;
        }

        public string BuildQuestionEndPoint(long id)
        {
            return $"{_webSiteEndPoint}question/{id}";
        }

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
            var nvc = new NameValueCollection
            {
                ["url"] = url,
                ["host"] = host,

            };
            if (location.HasValue)
            {
                nvc["location"] = location.ToString();
            }

            var uri = new UriBuilder(new Uri(_webSiteEndPoint))
            {
                Path = "url"
            }; // /*new Uri("/url",UriKind.Relative)*/);
            uri.AddQuery(nvc);
            return uri.ToString();
        }

        public string WalletEndPoint => _webSiteEndPoint + "wallet";

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
