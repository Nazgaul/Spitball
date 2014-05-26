namespace Zbang.Zbox.ViewModel.DTOs.Qna
{
    public class QuestionToFriendDto
    {
        private string m_BoxPicture;
        public string BoxPicutre
        {
            get
            {
                return m_BoxPicture;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                   // var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
                    m_BoxPicture = value;// blobProvider.GetThumbnailUrl(value);
                }
            }
        }
        public string BoxName { get; set; }
        public string Content { get; set; }
        public long Boxid { get; set; }
        public int AnswersCount { get; set; }


        public string UniversityName { get; set; }

        public string Url { get; set; }
    }

}
