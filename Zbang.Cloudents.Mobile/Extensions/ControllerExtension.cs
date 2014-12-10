using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public static class ControllerExtension
    {
        public static JsonNetResult CdJson(this Controller controller, object data)
        {
            return new JsonNetResult
            {
                Data = data
            };

        }


    }
}