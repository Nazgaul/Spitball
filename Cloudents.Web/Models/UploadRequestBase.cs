using Cloudents.Web.Binders;
using Newtonsoft.Json;

namespace Cloudents.Web.Models
{
    [JsonConverter(typeof(UploadRequestJsonConverter))]
    public class UploadRequestBase
    {

    }
}