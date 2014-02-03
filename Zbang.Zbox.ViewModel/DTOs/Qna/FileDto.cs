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
            m_Thumbnail = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailUrl(thumbnail);
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
