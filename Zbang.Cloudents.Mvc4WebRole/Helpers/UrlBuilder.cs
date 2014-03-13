using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class UrlBuilder
    {
        private UrlHelper _urlHelper;
        public UrlBuilder(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            _urlHelper = new UrlHelper(httpContext.Request.RequestContext);

        }
        public static string NameToQueryString(string name)
        {
            // - < > " ' % ; ) ( & + - 

            //<,>,*,%,&,:,\\
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }
            char previousChar = '\0';
            var sb = new StringBuilder();
            foreach (var character in name)
            {
                switch (character)
                {
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
                    case '\'':
                        continue;
                    case ' ':
                    case '_':
                    case '-':
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
            return sb.ToString().ToLower();

        }

        public string BuildBoxUrl(BoxType boxtype, long boxid, string boxName, string uniName)
        {
            if (boxtype == BoxType.Academic)
            {

                return _urlHelper.RouteUrl("CourseBox", new
                {
                    universityName = UrlBuilder.NameToQueryString(uniName),
                    boxId = boxid,
                    boxName = UrlBuilder.NameToQueryString(boxName)
                });

            }
            else
            {
                return _urlHelper.RouteUrl("PrivateBox", new { boxId = boxid, boxName = UrlBuilder.NameToQueryString(boxName) });

            }
        }

        public string BuildBoxUrl(long boxid, string boxName, string uniName)
        {
            if (!string.IsNullOrEmpty(uniName) && uniName != "my")
            {
                return BuildBoxUrl(BoxType.Academic, boxid, boxName, uniName);
            }
            else
            {
                return BuildBoxUrl(BoxType.Box, boxid, boxName, uniName);
            }
        }

        public string buildItemUrl(long boxId, string boxName, long itemId, string itemName, string universityName = "my")
        {
            if (string.IsNullOrEmpty(universityName))
            {
                universityName = "my";
            }
            return _urlHelper.RouteUrl("Item", new
            {
                universityName = UrlBuilder.NameToQueryString(universityName),
                boxId = boxId,
                boxName = UrlBuilder.NameToQueryString(boxName),
                itemid = itemId,
                itemName = UrlBuilder.NameToQueryString(itemName)
            });
        }

        public string BuildUserUrl(long userid, string userName)
        {
            return _urlHelper.RouteUrl("User", new
            {
                userId = userid,
                userName = UrlBuilder.NameToQueryString(userName)
            });
        }

        public string BuildDownloadUrl(long boxId, long itemId)
        {
            return _urlHelper.RouteUrl("ItemDownload", new
            {
                BoxUid = boxId,
                itemId = itemId
            });
        }
    }
}