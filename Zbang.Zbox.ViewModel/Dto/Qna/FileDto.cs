using System;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class FileDto : ItemDto
    {

        public FileDto(long id, long ownerId,
           string thumbnail, Guid? questionId, Guid? answerId, string name, string url)
            : base(id, ownerId, questionId, answerId, name, url)
        {

            Thumbnail = thumbnail;
        }

        //public override string Thumbnail
        //{
        //    get { return m_Thumbnail; }
        //    set { m_Thumbnail = value; }
        //}

        //public long Size { get; private set; }

        //public override string Type
        //{
        //    get { return "File"; }
        //}
    }
}
