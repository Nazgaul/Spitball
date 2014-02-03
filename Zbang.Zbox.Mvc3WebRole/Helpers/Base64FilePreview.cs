using System;

namespace Zbang.Zbox.Mvc3WebRole.Helpers
{
    public class Base64FilePreview
    {
        public string FileContent { get; private set; }
        public string ContentType { get; private set; }

        public Base64FilePreview(byte[] fileContent, string contentType)
        {
            ContentType = contentType;
            FileContent = TransformContentToBase64(fileContent);
        }

        private string TransformContentToBase64(byte[] content)
        {
            return Convert.ToBase64String(content);
        }
    }
}