namespace Zbang.Zbox.ViewModel.DTOs.Emails
{
    public class ItemDigestDto
    {
        
        private string m_Picture;
        public string UserName { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Picture { get { return m_Picture; }
            set
            {
                //var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
                m_Picture = value;// blobProvider.GetThumbnailUrl(value);
            }
        }
        public long UserId { get; set; }

    }
}
