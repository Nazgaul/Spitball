using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class ETagAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.IsChildAction) return;

            bool skipEtag = filterContext.ActionDescriptor.IsDefined(typeof(NoEtagAttribute), inherit: true)
                                    || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(NoEtagAttribute), inherit: true);
            if (skipEtag) return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(acceptEncoding)) return;
            acceptEncoding = acceptEncoding.ToUpperInvariant();
            HttpResponseBase response = filterContext.HttpContext.Response;

            if (response.Filter == null) return;
            if (!acceptEncoding.ToUpper().Contains("GZIP")) return;
            response.AppendHeader("Content-encoding", "gzip");
            response.Filter = new ETagFilter(
                response,
                filterContext.RequestContext.HttpContext.Request
                );
            //ie issue
            //if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            //{
            //    response.Cache.SetCacheability(HttpCacheability.NoCache);
            //    response.Cache.SetNoStore();
            //    response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            //}
        }


    }

    public class ETagFilter : GZipStream
    {
        private readonly HttpResponseBase m_Response;
        private readonly HttpRequestBase m_Request;
        //private readonly Stream m_Filter;
        private readonly MD5 m_Md5;
        private bool m_FinalBlock;



        public ETagFilter(HttpResponseBase response, HttpRequestBase request)
            : base(response.Filter, CompressionMode.Compress)
        {
            m_Response = response;
            m_Request = request;
            m_Md5 = MD5.Create();
        }


        protected override void Dispose(bool disposing)
        {
            m_Md5.Dispose();
            base.Dispose(disposing);
        }

        private string ByteArrayToString(byte[] arrInput)
        {
            var output = new StringBuilder(arrInput.Length);
            foreach (byte t in arrInput)
            {
                output.Append(t.ToString("X2"));
            }
            return output.ToString();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var str = Encoding.UTF8.GetString(buffer);
            var x = new Regex(@"<!--Donut#(.*)#-->", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            str = x.Replace(str, string.Empty);
            var buffer2 = Encoding.UTF8.GetBytes(str);
            m_Md5.TransformBlock(buffer2, 0, buffer2.Length, null, 0);
            base.Write(buffer2, 0, buffer2.Length);
        }

        public override void Flush()
        {
            if (m_FinalBlock)
            {
                base.Flush();
                return;
            }
            m_FinalBlock = true;
            m_Md5.TransformFinalBlock(new byte[0], 0, 0);
            var token = ByteArrayToString(m_Md5.Hash);
            string clientToken = m_Request.Headers["If-None-Match"];

            if (token != clientToken)
            {
                m_Response.Headers["ETag"] = token;
            }
            else
            {
                m_Response.SuppressContent = true;
                m_Response.StatusCode = 304;
                m_Response.StatusDescription = "Not Modified";
                m_Response.Headers["Content-Length"] = "0";

            }
            base.Flush();
        }
    }

}