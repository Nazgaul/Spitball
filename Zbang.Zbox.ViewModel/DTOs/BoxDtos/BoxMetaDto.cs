

namespace Zbang.Zbox.ViewModel.DTOs.BoxDtos
{
    public class BoxMetaDto
    {
        private string m_Image;

        public string Name { get; private set; }
        public long Id { get; private set; }

        public string UnivtesityName { get; set; }

        public string Url { get; set; }

        public string Image
        {
            get { return m_Image; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    m_Image = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailUrl(value);
                }
            }
        }

    }
}
