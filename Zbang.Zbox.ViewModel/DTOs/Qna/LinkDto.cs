using System;

namespace Zbang.Zbox.ViewModel.DTOs.Qna
{
    public class LinkDto : ItemDto
    {
        public LinkDto(long uid, long ownerId,
            string boxUid, Guid? questionId, Guid? answerId, string name, string thumbnail)
            : base(uid, ownerId, boxUid, questionId, answerId, name)
        {
            Thumbnail = thumbnail;
                // Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>().GetThumbnailLinkUrl();
            //m_Thumbnail = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailLinkUrl();
        }

        //public override string Thumbnail
        //{
        //    get
        //    {
        //        return m_Thumbnail;
        //    }
        //    set
        //    {
        //        m_Thumbnail = value;
        //    }

        //}

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
