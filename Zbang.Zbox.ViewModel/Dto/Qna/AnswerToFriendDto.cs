namespace Zbang.Zbox.ViewModel.DTOs.Qna
{
    public class AnswerToFriendDto
    {
        private string m_BoxPicture;
        public string BoxPicture
        {
            get
            {
                return m_BoxPicture;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    //  var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
                    m_BoxPicture = value;// blobProvider.GetThumbnailUrl(value);
                }
            }
        }
        public long Boxid { get; set; }
        public string BoxName { get; set; }
        public long QUserId { get; set; }
        public string QContent { get; set; }
        public string QUserImage { get; set; }
        public string QUserName { get; set; }
        public string Content { get; set; }
        public int AnswersCount { get; set; }


        public string Url { get; set; }

    }
}
