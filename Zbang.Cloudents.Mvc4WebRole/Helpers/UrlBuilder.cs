using System;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class UrlBuilder
    {
        private readonly UrlHelper m_UrlHelper;
        public UrlBuilder(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            m_UrlHelper = new UrlHelper(httpContext.Request.RequestContext);

        }
        public static string NameToQueryString(string name)
        {
            return UrlConsts.NameToQueryString(name);
        }

        private string BuildBoxUrl(BoxType boxtype, long boxid, string boxName, string uniName)
        {
            if (boxtype == BoxType.Academic)
            {
                return m_UrlHelper.RouteUrl("CourseBox", new
                {
                    universityName = NameToQueryString(uniName),
                    boxId = boxid,
                    boxName = NameToQueryString(boxName)
                });

            }
            return m_UrlHelper.RouteUrl("PrivateBox", new { boxId = boxid, boxName = NameToQueryString(boxName) });
        }

        public string BuildBoxUrl(long boxid, string boxName, string uniName)
        {
            if (!string.IsNullOrEmpty(uniName) && uniName != "my")
            {
                return BuildBoxUrl(BoxType.Academic, boxid, boxName, uniName);
            }
            return BuildBoxUrl(BoxType.Box, boxid, boxName, uniName);
        }

        //public string BuildQuizUrl(long boxId, string boxName, long quizId, string quizName, string universityName)
        //{
        //    if (string.IsNullOrEmpty(universityName))
        //    {
        //        universityName = "my";
        //    }
        //    return m_UrlHelper.RouteUrl("Quiz", new
        //    {
        //        universityName = NameToQueryString(universityName), boxId,
        //        boxName = NameToQueryString(boxName), quizId,
        //        quizName = NameToQueryString(quizName)
        //    });
        //}

        //public string BuildItemUrl(long boxId, string boxName, long itemId, string itemName, string universityName = "my")
        //{
        //    if (string.IsNullOrEmpty(universityName))
        //    {
        //        universityName = "my";
        //    }
        //    return m_UrlHelper.RouteUrl("Item", new
        //    {
        //        universityName = NameToQueryString(universityName), boxId,
        //        boxName = NameToQueryString(boxName),
        //        itemid = itemId,
        //        itemName = NameToQueryString(itemName)
        //    });
        //}

        //public string BuildUserUrl(long userid, string userName)
        //{
        //    return m_UrlHelper.RouteUrl("User", new
        //    {
        //        userId = userid,
        //        userName = NameToQueryString(userName)
        //    });
        //}

        public string BuildDownloadUrl(long boxId, long itemId)
        {
            return m_UrlHelper.RouteUrl("ItemDownload", new
            {
                BoxUid = boxId, itemId
            });
        }
    }
}