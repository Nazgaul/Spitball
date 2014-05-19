using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Qna
{
    public class FileDto : ItemDto
    {
        private string m_Thumbnail;

        public FileDto(long uid, long ownerId,
           string thumbnail, string boxUid, Guid? questionId, Guid? answerId, string name)
            : base(uid, ownerId, boxUid, questionId, answerId, name)
        {
            //TODO: this is not good should be logic in dto
            var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();

            m_Thumbnail = blobProvider.GetThumbnailUrl(thumbnail);
            //Size = size;
        }

        public override string Thumbnail
        {
            get { return m_Thumbnail; }
        }

        //public long Size { get; private set; }

        public override string Type
        {
            get { return "File"; }
        }
    }
}
