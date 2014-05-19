using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Qna
{
    public class LinkDto : ItemDto
    {
        private string m_Thumbnail;
        public LinkDto(long uid, long ownerId,
            string boxUid, Guid? questionId, Guid? answerId, string name)
            : base(uid, ownerId, boxUid, questionId, answerId, name)
        {
            m_Thumbnail = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>().GetThumbnailLinkUrl();
            //m_Thumbnail = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailLinkUrl();
        }

        public override string Thumbnail
        {
            get
            {
                return m_Thumbnail;
            }

        }

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
