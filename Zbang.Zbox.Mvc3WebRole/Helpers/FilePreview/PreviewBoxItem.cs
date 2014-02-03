using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.Infrastructure.Storage;
using System.Drawing;

namespace Zbang.Zbox.Mvc3WebRole.Helpers.FilePreview
{
    public abstract class PreviewBoxItem
    {
        protected FileDto FileData;
        protected IBlobProvider BlobProvider;

        protected PreviewBoxItem(FileDto fileData, IBlobProvider blobProvider)
        {
            FileData = fileData;
            BlobProvider = blobProvider;
        }

        public abstract string PreviewItem(Size userScreenResolution);
    }
}

