using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class CometResult : ActionResult
    {
       // private readonly ;
        private readonly HttpResponseBase m_Context;
        private int i = 0;
             
        public CometResult(ControllerContext context)
        {
            m_Context = context.HttpContext.Response;
            m_Context.Buffer = false;
            m_Context.ContentType = "application/json";
            
            //m_Context.SupportsAsyncFlush = true;

        }
        public void PushData(object data)
        {
            JavaScriptSerializer m_Serializer = new JavaScriptSerializer();
            var obj = new { chunk = i, obj = data };
            var serializedObj = m_Serializer.Serialize(obj);
            m_Context.Write(serializedObj);
            i++;
            m_Context.Flush();
            //System.Threading.Thread.Sleep(5);
        }


        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.End();
        }
    }
}