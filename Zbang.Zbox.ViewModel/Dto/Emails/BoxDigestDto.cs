namespace Zbang.Zbox.ViewModel.Dto.Emails
{
    public class BoxDigestDto
    {
        private string m_Picture;
        public long BoxId { get; set; }
        public string BoxName { get; set; }
        public string BoxPicture
        {
            get { return m_Picture; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    m_Picture = value;
                }
            }
        }
        public string UniversityName { get; set; }

        public string Url { get; set; }

    }
}
