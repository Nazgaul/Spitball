using System;

namespace Zbang.Zbox.ViewModel.DTOs.Qna
{
    public class FileDto : ItemDto
    {

        public FileDto(long uid, long ownerId,
           string thumbnail, string boxUid, Guid? questionId, Guid? answerId, string name, string url)
            : base(uid, ownerId, boxUid, questionId, answerId, name, url)
        {

            Thumbnail = thumbnail;
        }

        //public override string Thumbnail
        //{
        //    get { return m_Thumbnail; }
        //    set { m_Thumbnail = value; }
        //}

        //public long Size { get; private set; }

        public override string Type
        {
            get { return "File"; }
        }
    }
}
