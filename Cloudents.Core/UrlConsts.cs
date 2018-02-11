using System;
using System.Collections.Specialized;
using System.Text;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core
{
    public class UrlConst : IUrlBuilder
    {
        //private const string SystemUrl = "https://www.spitball.co";
        private readonly string _systemUrl;

        public UrlConst(string systemUrl)
        {
            _systemUrl = systemUrl;
        }

        public string BuildRedirectUrl(string url, string host, int? location)
        {
            var nvc = new NameValueCollection
            {
                ["url"] = url,
                ["host"] = host,

            };
            if (location.HasValue)
            {
                nvc["location"] = location.ToString();
            }
            var uri = new UriBuilder(new Uri(_systemUrl))
            {
                Path = "url"
            };
            uri.AddQuery(nvc);
            return uri.ToString();
        }

        public static string NameToQueryString(string name)
        {
            //var outString =  sourceString.replace(/[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi, '');
            // - < > " ' % ; ) ( & + -

            //<,>,*,%,&,:,\\
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

    public static class MetadataConst
    {
        public const string VideoStatus = "VideoStatus";
    }
}
