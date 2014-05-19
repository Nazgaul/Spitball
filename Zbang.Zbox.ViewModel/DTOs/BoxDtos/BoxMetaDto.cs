

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
                    var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
                    m_Image = blobProvider.GetThumbnailUrl(value);
                }
            }
        }

    }
}
