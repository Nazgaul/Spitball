using Cloudents.Admin2.Binders;
using Newtonsoft.Json;

namespace Cloudents.Admin2.Models
{

    [JsonConverter(typeof(UploadRequestJsonConverter))]
    public class UploadRequestBase
    {

    }
}
