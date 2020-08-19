using System;
using Cloudents.Web.Models;
using Newtonsoft.Json.Linq;

namespace Cloudents.Web.Binders
{
    public class UploadRequestJsonConverter : JsonCreationConverter<UploadRequestBase>
    {
        protected override UploadRequestBase Create(Type objectType, JObject jObject)
        {
            if (jObject == null) throw new ArgumentNullException(nameof(jObject));

            var phaseStr = jObject.GetValue("phase", StringComparison.OrdinalIgnoreCase)?.Value<string>();

            if (Enum.TryParse(phaseStr, true, out UploadPhase phase))
            {
                if (phase == UploadPhase.Start)
                {
                    return new UploadRequestStart();
                }

                if (phase == UploadPhase.Finish)
                {
                    if (jObject["conversationId"] != null)
                    {
                        return new FinishChatUpload();
                    }

                    return new UploadRequestFinish();
                }
            }

            throw new ArgumentException();
        }
    }
}