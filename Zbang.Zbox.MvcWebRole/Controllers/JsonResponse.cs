
namespace Zbang.Zbox.MvcWebRole.Controllers
{
    public class JsonResponse
    {
        public JsonResponse(bool success)
        {
            this.Success = success;
           
        }

        public JsonResponse(bool success, object payload): this(success)
        {
            this.Payload = payload;
        }

        public bool Success { get; private set; }
        public object Payload { get; set; }
    }
}