using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.DTOs.Emails
{
    public class BoxDigestDto
    {
        private string m_Picture;
        public long BoxId { get; set; }
        public string BoxName { get; set; }
        public string BoxPicture
        {
            get { return m_Picture; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    m_Picture = "https://www.cloudents.com/images/emptyState/my_default3.png";
                }
                else
                {
                    var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
                    m_Picture = blobProvider.GetThumbnailUrl(value);
                }
            }

        }
        public string UniversityName { get; set; }

        public string Url { get; set; }

    }
}
