using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jil;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public class JilJsonResult : JsonResult
    {

        private static Options JilOptions()
        {
            return new Options(
                    serializationNameFormat: SerializationNameFormat.CamelCase,
                    excludeNulls: true,
                    dateFormat: DateTimeFormat.ISO8601,
                    unspecifiedDateTimeKindBehavior: UnspecifiedDateTimeKindBehavior.IsUTC);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";

            response.Write(JSON.Serialize(Data, JilOptions()));
            //JSON.Serialize(Data, response.Output);
            //response.Flush();
        }
    }
}