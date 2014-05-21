using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.DTOs.BoxDtos
{
    public class BoxToFriendDto
    {
        private string m_Picture;

        public UserRelationshipType UserType { get; set; }
        public long Id { get; set; }
        public string Universityname { get; set; }
        public string Name { get; set; }
        public string Picture
        {
            get
            {
                return m_Picture;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    //var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
                    m_Picture = value;// blobProvider.GetThumbnailUrl(value);
                }
            }
        }

        public string Url { get; set; }
       
    }
}
