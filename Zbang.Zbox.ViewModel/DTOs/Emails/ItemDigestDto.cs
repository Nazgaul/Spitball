using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                m_Picture = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailUrl(value);
            }
        }
        public long UserId { get; set; }

    }
}
