using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class ETagAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            
            var stream = filterContext.HttpContext.Response.OutputStream;
            var etag = GetToken(stream);

            base.OnResultExecuted(filterContext);
        }

        private string GetToken(Stream stream)
        {
            MD5 md5 = MD5.Create();
            byte[] checksum = md5.ComputeHash(stream.ConvertToByteArray());
             //md5.ComputeHash(stream);
            return Convert.ToBase64String(checksum);
        }
    }
}