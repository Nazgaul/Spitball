using System;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    [Serializable]
    public class UserDto
    {
        private string m_Name;
        public string Image { get; set; }
        public string LargeImage { get; set; }
        public string Name
        {
            get { return m_Name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    m_Name = value.Trim();
                }
            }
        }
        public long Uid { get; set; }

        public string Url { get; set; }

    }

    public class UserMinProfile
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
        public string UniversityName { get; set; }

        public string Url { get; set; }
    }
}
